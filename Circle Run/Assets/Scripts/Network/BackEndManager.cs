using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Game.Rank;
using Newtonsoft.Json;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
#endif

public class BackEndManager : MonoBehaviour
{
    public static BackEndManager Instance;
    public static string myNickName
    {
        get
        {
            if (Backend.UserNickName == null)
                return null;
                
            if (Backend.UserNickName.Length > 0)
                return Backend.UserNickName;
            else
                return null;
        }
    }
    public static bool isInit = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
       .Builder()
       .RequestServerAuthCode(false)
       .RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요.
       .RequestIdToken()
       .Build();
        //커스텀 된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
        PlayGamesPlatform.Activate();
    }
    private void BackEndDataInitInsert()
    {
        SendQueue.Enqueue(Backend.GameData.Insert, "UserData", (callback) =>
        {
            Debug.Log("SendQueue Success");
        });
        SendQueue.Enqueue(Backend.GameData.Insert, "UserItemData", (callback) =>
        {
            Debug.Log("SendQueue Success");
        });
    }
    private void BackEndDataInit()
    {
        SendQueue.Enqueue(Backend.GameData.GetMyData, "UserData", new Where(), 1, (callback) =>
        {
            if (callback.IsSuccess())
            {
                string json = callback.GetFlattenJSON().ToJson();
                DataManager.userScore = JsonUtility.FromJson<UserScore>(json);
            }
            Debug.Log("SendQueue Success");
        });
        SendQueue.Enqueue(Backend.GameData.GetMyData, "UserItemData", new Where(), 1, (callback) =>
        {
            if (callback.IsSuccess())
            {
                string json = callback.GetFlattenJSON().ToJson();
                DataManager.userItem = JsonUtility.FromJson<UserItem>(json);
            }
        });
    }
    #region Google
    public void GPGSLogin(bool result)
    {
        // 이미 로그인 된 경우
        if (result)
        {
            Debug.Log("result  =" + result);
            string token = GetTokens();
            Backend.BMember.CheckUserInBackend(token, FederationType.Google, (bro) =>
            {
                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(token, FederationType.Google, "gpgs");
                if (bro.IsSuccess())
                    BackEndDataInitInsert();
                else
                    BackEndDataInit();
            });
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                Debug.Log("Login Success  =" + success);
                if (success)
                {
                    Backend.BMember.CheckUserInBackend(GetTokens(), FederationType.Google, (bro) =>
                    {
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
                        if (bro.IsSuccess())
                            BackEndDataInitInsert();
                        else
                            BackEndDataInit();

                    });
                    // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청

                }
                else
                {
                    // 로그인 실패
                    Debug.Log("Login failed for some reason");
                }
            });
        }
    }
    public string GetTokens()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            // 유저 토큰 받기 첫 번째 방법
            string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
            // 두 번째 방법
            // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
            return _IDtoken;
        }
        else
        {
            Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return null;
        }
    }
    #endregion
    #region Apple
    #endregion
    #region Version&Init
    public bool Init()
    {
        isInit = true;
        bool isResult = true;
        var bro = Backend.Initialize(true, true);
        if (bro.IsSuccess())
        {
            isResult = VersionCheck();

            var login = Backend.BMember.LoginWithTheBackendToken();
            if (!login.IsSuccess())
            {
#if UNITY_EDITOR
                Backend.BMember.GuestLogin();
#else
        PlayGamesPlatform.Instance.Authenticate(GPGSLogin);
#endif
                SendQueue.Enqueue(Backend.GameData.Insert, "UserData", (callback) =>
                {
                    Debug.Log("SendQueue Success");
                });
                SendQueue.Enqueue(Backend.GameData.Insert, "UserItemData", (callback) =>
                {
                    Debug.Log("SendQueue Success");
                });
            }
            else
                BackEndDataInit();
        }
        else
        {
        }
        return isResult;
    }
    private bool VersionCheck()
    {
        bool isResult = true;
#if !UNITY_EDITOR

        var bro = Backend.Utils.GetLatestVersion();
        if (bro.IsSuccess())
        {
            string serverVersion = bro.GetReturnValuetoJSON()["version"].ToString();

            if (serverVersion == Application.version)
            { }  //버전 일치 다음 진행 
            string forceUpdate = bro.GetReturnValuetoJSON()["type"].ToString();
            if (forceUpdate == "1")  // 선택적 업데이트
            { }
            else if (forceUpdate == "2")  // 강제 업데이트 
            { }
        }
        else
        {
            //네트워크 체크 및 다시 시도버튼을 띄원다. 
            isResult = false;
            Debug.LogError("버전 정보 조회 실패");
        }
#endif
        return isResult;
    }
    #endregion
    #region User
    public void SetNickName(string nickName, System.Action<bool, string> callback)
    {
        var bro = Backend.BMember.UpdateNickname(nickName);
        callback?.Invoke(bro.IsSuccess(), bro.GetErrorCode());
    }
    public bool NickNameCheck(string nickName)
    {
        bool isResult = false;
        var bro = Backend.BMember.CheckNicknameDuplication(nickName);
        if (bro.IsSuccess())
        {
            isResult = true;
        }
        else
        {
            switch (bro.GetErrorCode())
            {
                case "BadParameterException":
                    //20자 이상 및 닉네임에 공백이 존재
                    break;
                case "DuplicatedParameterException":
                    //중복
                    break;
            }
        }
        return isResult;
    }
    public bool AutoLogin()
    {
        bool isResult = false;
        var bro = Backend.BMember.LoginWithTheBackendToken();
        if (bro.IsSuccess())
        {
            Debug.Log("Auto Login");
            isResult = true;
        }
        return isResult;
    }
    public void Logout()
    {
        Backend.BMember.Logout((callback) =>
        {
            if (callback.IsSuccess())
            { }
            else
            { }
        });
    }
    #endregion
    #region Ranking
    public void GetRanking()
    {
        SendQueue.Enqueue(Backend.URank.User.GetRankList, GetRankingTable(Ranking.Daily), 50, (bro) =>
        {
            if (bro.IsSuccess())
            {
                string json = bro.GetFlattenJSON().ToJson();
                RankList data = JsonConvert.DeserializeObject<RankList>(json);
                DataManager.dailyRanking = data;
            }
        });
        SendQueue.Enqueue(Backend.URank.User.GetRankList, GetRankingTable(Ranking.Week), 5, (bro) =>
        {
            if (bro.IsSuccess())
            {
                string json = bro.GetFlattenJSON().ToJson();
                RankList data = JsonConvert.DeserializeObject<RankList>(json);
                DataManager.weekRanking = data;
            }
        });
    }
    private void RankingDataSet(RankList data, Ranking ranking)
    {
        switch (ranking)
        {
            case Ranking.Daily:
                DataManager.dailyRanking = data;
                break;
            case Ranking.Week:
                DataManager.weekRanking = data;
                break;
        }
    }
    public void RankingUpdate(Ranking ranking, int score)
    {
        string table = GetRankingTable(ranking);
        Param param = new Param();
        if(ranking == Ranking.Daily)
            param.Add("DailyScore", score);
        else
            param.Add("weekScore", score);
        var bro = Backend.URank.User.UpdateUserScore(table, "UserData", DataManager.userScore.inDate, param);
        if (bro.IsSuccess())
        {
            Debug.Log("Success");
        }
        else
        {
            Debug.Log("Fail : " + bro.GetMessage());
            //랭킹 다시 시도
        }
        LoadingManager.Instance.LoadingStop();
    }
    private string GetRankingTable(Ranking ranking)
    {
        string table = Define.Ranking_Daily;
        switch (ranking)
        {
            case Ranking.Week:
                table = Define.Ranking_Week;
                break;
        }
        return table;
    }
    #endregion
    #region GameData
    public BackEndGameData<T> GetGameData<T>(string tableName) where T : BackEndBase, new()
    {
        var bro = Backend.GameData.GetMyData(tableName, new Where(),1);
        BackEndGameData<T> data = new BackEndGameData<T>();
        if (bro.IsSuccess())
        {
            string json = bro.GetFlattenJSON().ToJson();
            data = JsonConvert.DeserializeObject<BackEndGameData<T>>(json); //JsonUtility.FromJson<T>(json);
        }
        else
        {
            //게임데이터 불러오기 에러, 팝업 이나 문구 띄우고, 네트워크 상태 체크
        }
        return data;
    }
    public void GameDataInsert(string tableName)
    {
        var bro = Backend.GameData.Insert(tableName);
        if (bro.IsSuccess())
        { }
        else
        {
            //게임데이터 추가 에러, 팝업 이나 문구 띄우고, 네트워크 상태 체크
        }
    }
    public void UseContinueCoupon(System.Action<bool> callback)
    {
        LoadingManager.Instance.LoadingStart();
        UserItem item = DataManager.userItem;
        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic.Add("continueCoupon", item.continueCoupon - 1);
        GameDataUpdate("UserItemData", item.inDate, dic, callback);
    }
    public void UseShield()
    {
        Param param = new Param();
        UserItem item = DataManager.userItem;
        --DataManager.userItem.shield;
        param.Add("shield", item.shield-1);
        SendQueue.Enqueue(Backend.PlayerData.UpdateMyData, "UserItemData", item.inDate, param, (callback) => { 
            if(callback.IsSuccess())
            { }
            else
            { }
        });
    }
    public void ScoreUpdate()
    {
        LoadingManager.Instance.LoadingStart();
        Param param = new Param();
        UserScore score = DataManager.userScore;
        param.Add("DailyScore", score.DailyScore);
        param.Add("weekScore", score.weekScore);
        RankingUpdate(Ranking.Daily, score.DailyScore);
        RankingUpdate(Ranking.Week, score.weekScore);
        //SendQueue.Enqueue(Backend.PlayerData.UpdateMyData, "UserData", score.inDate, param, (callback) => {
        //    LoadingManager.Instance.LoadingStop();

        //});
    }
    public void GameDataUpdate(string tableName, string inDate, Dictionary<string, int> dic, Action<bool> callback)
    {
        Param param = new Param();
        foreach (var i in dic)
            param.Add(i.Key, i.Value);
        var result = Backend.PlayerData.UpdateMyData(tableName, inDate, param);
        bool isResult = result.IsSuccess();
        if (isResult)
            callback?.Invoke(isResult);
        else
        {
            Debug.Log("Error Message : " + result.GetMessage());
        }
        LoadingManager.Instance.LoadingStop();
    }
    #endregion
}
