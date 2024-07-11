using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Game.Rank;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
#endif

public class BackEndManager : MonoBehaviour
{
    public static BackEndManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
                                                  //GPGS 시작.
        PlayGamesPlatform.Activate();
    }
    private void DataInit()
    {
        SendQueue.Enqueue(Backend.GameData.GetMyData, "UserItem", Backend.UserInDate, callback =>
        {
            // 이후 처리
        });
    }
    #region Google
    public void GPGSLogin()
    {
        // 이미 로그인 된 경우
        if (Social.localUser.authenticated == true)
        {
            BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
        }
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // 로그인 성공 -> 뒤끝 서버에 획득한 구글 토큰으로 가입 요청
                    BackendReturnObject BRO = Backend.BMember.AuthorizeFederation(GetTokens(), FederationType.Google, "gpgs");
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
        bool isResult = true;
        var bro = Backend.Initialize(true, true);
        if (bro.IsSuccess())
        {
            isResult = VersionCheck();
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
    public void SetNickName(string nickName)
    {
        var bro = Backend.BMember.UpdateNickname(nickName);
        if (bro.IsSuccess())
        { }
        else
        { }
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
    public void GetRanking(Ranking ranking)
    {
        string table = GetRankingTable(ranking);
        var bro = Backend.URank.User.GetRankList(table, 50);
        if (bro.IsSuccess())
        {
            string json = bro.GetFlattenJSON().ToJson();
            UserRank[] userRank = JsonUtility.FromJson<UserRank[]>(json);
        }
        else
        { }
    }
    public void RankingUpdate(Ranking ranking, int score)
    {
        string table = GetRankingTable(ranking);
        Param param = new Param();
        param.Add("score", score);
        var bro = Backend.URank.User.UpdateUserScore(table, "", "", param);
    }
    private string GetRankingTable(Ranking ranking)
    {
        string table = Define.Ranking_Daily;
        switch (ranking)
        {
            case Ranking.Week:
                table = Define.Ranking_Week;
                break;
            case Ranking.Mon:
                table = Define.Ranking_Mon;
                break;
            case Ranking.Total:
                table = Define.Ranking_Total;
                break;
        }
        return table;
    }
    #endregion
    #region GameData
    public T GetGameData<T>(string tableName) where T : DataManager.BackEndBase, new()
    {
        var bro = Backend.GameData.GetMyData(tableName, Backend.UserInDate);
        T data = new T();
        if (bro.IsSuccess())
        {
            string json = bro.GetFlattenJSON().ToJson();
            data = JsonUtility.FromJson<T>(json);
        }
        else
        {
            //�ʱ�ȭ �� �μ�Ʈ
        }
        return data;
    }
    public void GameDataInsert(string tableName)
    {
        var bro = Backend.GameData.Insert(tableName);
        if (bro.IsSuccess())
        { }
        else
        { }
    }
    public void GameDataUpdate(string tableName, Dictionary<string, string> dic)
    {
        Param param = new Param();
        foreach (var i in dic)
            param.Add(i.Key, i.Value);
        Backend.GameData.UpdateV2(tableName, DataManager.userItem.inDate, Backend.UserInDate, param, (callback) =>
        {
        });
    }
    #endregion
}
