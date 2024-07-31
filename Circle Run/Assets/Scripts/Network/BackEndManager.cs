using System;
using System.Globalization;
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
using AppleAuth;
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
    private NetErrorUI netErrorUI;
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
#if UNITY_ANDROID
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
#endif
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
            var bro = Backend.BMember.CheckUserInBackend(token, FederationType.Google);
            Debug.LogError("BackEnd Login Try");
            if (bro.IsSuccess())
            {
                Debug.LogError("BackEnd CheckUser StatusCode : " + bro.GetStatusCode());
                BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(token, FederationType.Google, "gpgs");
                if (bro.GetStatusCode() == "204")
                {
                    Debug.Log("Authorize Result : " + BRO.IsSuccess() + "\n" + BRO.GetMessage());
                    if (BRO.IsSuccess())
                    {
                        isInit = true;
                        BackEndDataSetting();
                        BackEndDataInit();
                        TitleManager.Instance.UserDataInit();
                    }
                }
                else if (bro.GetStatusCode() == "200")
                {
                    isInit = true;
                    BackEndDataInit();
                    TitleManager.Instance.UserDataInit();
                }

            }
            else
            {
                Debug.Log("BackEnd Login Error : " + bro.GetErrorCode());
            }
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                Debug.Log("Login Success  =" + success);
                if (success)
                {
                    string token = GetTokens();
                    var bro = Backend.BMember.CheckUserInBackend(token, FederationType.Google);
                    if (bro.IsSuccess())
                    {
                        BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(token, FederationType.Google, "gpgs");
                        if (bro.GetStatusCode() == "204")
                        {
                            if (BRO.IsSuccess())
                            {
                                isInit = true;
                                BackEndDataSetting();
                                BackEndDataInit();
                                TitleManager.Instance.UserDataInit();
                            }
                        }
                        else if (bro.GetStatusCode() == "200")
                        {
                            isInit = true;
                            BackEndDataInit();
                            TitleManager.Instance.UserDataInit();
                        }
                    }
                    // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
                }
                else
                {
                    // 로그인 실패
                    LoadingManager.Instance.LoadingStop();
                    if (netErrorUI == null)
                        netErrorUI = Instantiate(Resources.Load<NetErrorUI>("Prefabs/UI/NetWorkErrorUI"));
                    netErrorUI.Init(() =>
                    {
                        LoadingManager.Instance.LoadingStart();
                        Init();
                    });
                    //TitleManager.Instance.LoginError();
                    Debug.LogError("Login failed for some reason");
                }
            });
        }
    }
    public string GetTokens()
    {
#if UNITY_ANDROID
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
#elif UNITY_IOS
#endif
    }
    #endregion
    #region Apple
    public void AppleLogin()
    {

    }
    public void AppleAuth()
    {
        BackendReturnObject bro = Backend.BMember.AuthorizeFederation("idToken", FederationType.Apple, "siwa");
        if (bro.IsSuccess())
        {
            Debug.Log("APPLE 로그인 성공");
            //성공 처리
        }
        else
        {
            Debug.LogError("Apple 로그인 실패");
            //실패 처리
        }
    }
    #endregion
    #region Version&Init
    public void Init()
    {
        Debug.Log("BackEnd Init");
        bool isResult = true;
        var bro = Backend.Initialize(true, true);
        if (bro.IsSuccess())
        {
            isResult = VersionCheck();
            Debug.Log("Init Result : " + isResult);
            if (!isResult)
            {
                //TitleManager.Instance.VersionCheckResult(isResult);
            }
            else
            {
                var login = Backend.BMember.LoginWithTheBackendToken();
                Debug.Log(login.IsSuccess() + " : Token Login Result");
                if (!login.IsSuccess())
                {
#if UNITY_EDITOR
                    Debug.Log("GeustLogin Start");
                    Backend.BMember.GuestLogin((callback) =>
                    {
                        Debug.Log("GestLogin Result : " + callback.IsSuccess());
                        if (callback.IsSuccess())
                        {
                            BackEndDataSetting();
                            BackEndDataInit();
                            TitleManager.Instance.UserDataInit();
                        }
                        else
                        {
                            Debug.Log("GuestLogin Fail : " + callback.GetMessage());
                            //씬을 다시 구성 
                        }
                    });
#else
        PlayGamesPlatform.Instance.Authenticate(GPGSLogin);
#endif
                }
                else
                {
                    isInit = true;
                    BackEndDataInit();
                    TitleManager.Instance.UserDataInit();
                }
            }
        }
        else
        {
            LoadingManager.Instance.LoadingStop();
            if (netErrorUI == null)
                netErrorUI = Instantiate(Resources.Load<NetErrorUI>("Prefabs/UI/NetWorkErrorUI"));
            netErrorUI.Init(() =>
            {
                LoadingManager.Instance.LoadingStart();
                Init();
            });
        }
    }
    private void BackEndDataSetting()
    {
        SendQueue.Enqueue(Backend.GameData.Insert, "UserData", (callback) =>
        {
            Debug.Log("SendQueue Success");
        });
        SendQueue.Enqueue(Backend.GameData.Insert, "UserItemData", (callback) =>
        {
            Debug.Log("SendQueue Success");
        });
        SendQueue.Enqueue(Backend.GameData.Insert, "TimeCheck", (callback) =>
        {
            Debug.Log("SendQueue Success");
        });
    }
    private bool VersionCheck()
    {
        bool isResult = true;
#if !UNITY_EDITOR

        var bro = Backend.Utils.GetLatestVersion();
        if (bro.IsSuccess())
        {
            string serverVersion = bro.GetReturnValuetoJSON()["version"].ToString();
            Debug.LogError("Version Server : "+serverVersion);
            Debug.LogError("Version App : "+Application.version);
            if (serverVersion == Application.version)
            { }  //버전 일치 다음 진행
            else
            {
            string forceUpdate = bro.GetReturnValuetoJSON()["type"].ToString();
            if (forceUpdate == "1")  // 선택적 업데이트
            { }
            else if (forceUpdate == "2")  // 강제 업데이트 
            { 
                
#if UNITY_ANDROID
Application.OpenURL("https://play.google.com/store/apps/details?id=com.novembernine.dongrami&hl=ko");
#else
#endif
            }
            }
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
    public void GetRankReward()
    {
        int t = 0;
        int e = 0;
        RankReward("Daily DailyReward", 1,ref t,ref e);
        int shield = 0;
        int coupon = 0;
        var bro = Backend.UPost.GetPostList(PostType.Rank, 10);
        if (bro.IsSuccess())
        {
            LitJson.JsonData json = bro.GetReturnValuetoJSON()["postList"];
            if (json.Count > 0)
            {

                Debug.Log(json.ToJson());
                for (int i = 0; i < json.Count; i++)
                {
                    string title = json[i]["title"].ToString();

                    int endIndex = title.IndexOf(")");
                    char num = title[title.Length - 2];

                    int rank = num - '0';
                    RankReward(title, rank, ref shield, ref coupon);
                }
                DataManager.userItem.shield += shield;
                DataManager.userItem.continueCoupon += coupon;
                TitleManager.Instance.ItemUISet();
                Dictionary<string, int> dic = new Dictionary<string, int>();
                dic.Add("shield", DataManager.userItem.shield);
                dic.Add("continueCoupon", DataManager.userItem.continueCoupon);
                GameDataUpdate("UserItemData", DataManager.userItem.inDate, dic, (result) =>
                {
                    if (result)
                    {
                        SendQueue.Enqueue(Backend.UPost.ReceivePostItemAll, PostType.Rank, (callback) =>
                      {
                      });
                    }
                });
                TitleManager.Instance.RankRewardPop(shield, coupon);
                LoadingManager.Instance.LoadingStop();
                //SendQueue.Enqueue(Backend.UPost.ReceivePostItemAll, PostType.Rank, (callback) =>
                //{
                //});
            }
        }
        LoadingManager.Instance.LoadingStop();
    }
    private void RankReward(string title, int rank, ref int shield, ref int coupon)
    {
        if (title.StartsWith("daily") || title.StartsWith("Daily"))
        {
            switch (rank)
            {
                case 1:
                    shield += 2;
                    coupon += 2;
                    break;
                case 2:
                    shield += 1;
                    coupon += 1;
                    break;
                case 3:
                    coupon += 1;
                    break;
                case 4:
                case 5:
                    shield += 1;
                    break;
            }
        }
        else if (title.StartsWith("week") || title.StartsWith("Week"))
        {
            switch (rank)
            {
                case 1:
                    shield += 10;
                    coupon += 10;
                    break;
                case 2:
                    shield += 7;
                    coupon += 7;
                    break;
                case 3:
                    shield += 5;
                    coupon += 5;
                    break;
            }
        }
    }
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
        if (ranking == Ranking.Daily)
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
        var bro = Backend.GameData.GetMyData(tableName, new Where(), 1);
        BackEndGameData<T> data = new BackEndGameData<T>();
        if (bro.IsSuccess())
        {
            string json = bro.GetFlattenJSON().ToJson();
            data = JsonConvert.DeserializeObject<BackEndGameData<T>>(json); //JsonUtility.FromJson<T>(json);
            Debug.Log(data.rows.Length);
            if (data.rows.Length == 0)
            {
                var insert = Backend.GameData.Insert(tableName);
                if (insert.IsSuccess())
                {
                    bro = Backend.GameData.GetMyData(tableName, new Where(), 1);
                    json = bro.GetFlattenJSON().ToJson();
                    data = JsonConvert.DeserializeObject<BackEndGameData<T>>(json);
                }
            }
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
    public void UseShield(int amount, Action<bool> callback)
    {
        Param param = new Param();
        UserItem item = DataManager.userItem;
        item.shield -= amount;
        param.Add("shield", item.shield);
        var res = Backend.PlayerData.UpdateMyData("UserItemData", item.inDate, param);
        callback?.Invoke(res.IsSuccess());

        //SendQueue.Enqueue(Backend.PlayerData.UpdateMyData, "UserItemData", item.inDate, param, (res) =>
        //{
        //    if (res.IsSuccess())
        //        callback?.Invoke();
        //    else
        //    {
        //        //에러 판넬
        //        LoadingManager.Instance.LoadingStop();
        //    }
        //});
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
        GetRanking();
        //SendQueue.Enqueue(Backend.PlayerData.UpdateMyData, "UserData", score.inDate, param, (callback) => {
        //    LoadingManager.Instance.LoadingStop();

        //});
    }
    /// <summary>
    /// 게임 데이터 업데이트
    /// </summary>
    /// <param name="tableName">테이블 명</param>
    /// <param name="inDate">테이블 InDate</param>
    /// <param name="dic">담을 내용</param>
    /// <param name="callback">콜백</param>
    public void GameDataUpdate(string tableName, string inDate, Dictionary<string, int> dic, Action<bool> callback)
    {
        Param param = new Param();
        foreach (var i in dic)
            param.Add(i.Key, i.Value);
        var result = Backend.PlayerData.UpdateMyData(tableName, inDate, param);
        bool isResult = result.IsSuccess();
        callback?.Invoke(isResult);
        LoadingManager.Instance.LoadingStop();

    }
    public void ItemDataUpdate(Action<bool> action)
    {
        var i = DataManager.userItem;
        Param param = new Param();
        param.Add("adsRemove", i.adsRemove);
        param.Add("continueCoupon", i.continueCoupon);
        param.Add("shield", i.shield);

        var bro = Backend.PlayerData.UpdateMyData("UserItemData", i.inDate, param);
        action?.Invoke(bro.IsSuccess());
    }
    #endregion
    #region Time
    public DateTime GetTime()
    {
        var bro = Backend.Utils.GetServerTime();
        if (bro.IsSuccess())
        {
            string time = bro.GetReturnValuetoJSON()["utcTime"].ToString();
            DateTime parsedDate = DateTime.Parse(time);
            // UTC 시간을 한국 시간대로 변환
            return parsedDate;
        }
        return DateTime.UtcNow;
    }
    /// <summary>
    /// 시간 업데이트
    /// </summary>
    /// <param name="param">업데이트할 시간</param>
    /// <param name="inDate">데이터 inDate</param>
    public void GetTimeUpdate(Param param, string inDate, Action callback = null)
    {
        var bro = Backend.PlayerData.UpdateMyData("TimeCheck", inDate, param);
        if (bro.IsSuccess())
        {
            callback?.Invoke();
        }
        else
        { }
        Debug.Log(bro.GetMessage());
        
    }
    public void SendQueueTimeUpdate(Param param, string inDate)
    {
        SendQueue.Enqueue(Backend.PlayerData.UpdateMyData, "TimeCheck", inDate, param, (result) =>
        {
            if (result.IsSuccess())
            { }
            else
            { }
            Debug.Log(result.GetMessage());
        });
    }
    #endregion
}
