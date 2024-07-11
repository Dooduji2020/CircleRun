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
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    private void DataInit()
    {
        SendQueue.Enqueue(Backend.GameData.GetMyData, "UserItem", Backend.UserInDate,callback =>
        {
            // 이후 처리
        });
    }
#region Google
    void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            GetAccessCode();
            // Continue with Play Games Services
        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        }
    }
    public void GetAccessCode()
    {
        PlayGamesPlatform.Instance.RequestServerSideAccess(
          /* forceRefreshToken= */ false,
          code => {
              Debug.Log("구글 인증 코드 : " + code);

              Backend.BMember.GetGPGS2AccessToken(code, googleCallback =>
              {
                  Debug.Log("GetGPGS2AccessToken 함수 호출 결과 " + googleCallback);

                  string accessToken = "";

                  if (googleCallback.IsSuccess())
                  {
                      accessToken = googleCallback.GetReturnValuetoJSON()["access_token"].ToString();
                      Backend.BMember.AuthorizeFederation(accessToken, FederationType.GPGS2, callback =>
                      {
                          Debug.Log("뒤끝 로그인 성공했습니다. " + callback);
                          DontDestroyOnLoad(gameObject);
                          TitleManager.Instance.UserDataInit();
                      });
                  }
              });
          });
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
        Backend.BMember.Logout((callback) => { 
            if(callback.IsSuccess())
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
