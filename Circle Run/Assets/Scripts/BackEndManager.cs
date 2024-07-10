using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using BackEnd.Game.Rank;
using UnityEditor.MPE;

public class BackEndManager : MonoBehaviour
{
    public static BackEndManager Instance;
    private UserItem userItem = new UserItem();
    public delegate void ExceptionEvent(Exception e);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
            Destroy(this.gameObject);
    }
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
    public void GetRanking(Ranking ranking)
    {
        string table = GetRankingTable(ranking);
        var bro = Backend.URank.User.GetRankList(table, 50);
        if (bro.IsSuccess())
        {
            string json = bro.GetFlattenJSON().ToJson();
            UserRank userRank = new UserRank();
        }
        else
        { }
    }
    public void SetNickName(string nickName)
    {
        var bro = Backend.BMember.UpdateNickname(nickName);
        if (bro.IsSuccess())
        { }
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
    public T GetGameData<T>(string tableName) where T : BackEndBase, new()
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
        Backend.GameData.UpdateV2(tableName, userItem.inDate, Backend.UserInDate, param, (callback) =>
        {
        });
    }
}
public enum Ranking
{
    Daily,
    Week,
    Mon,
    Total
}
public class BackEndBase
{
    public string inDate;
    public string owner_inDate;
}
public class UserScore : BackEndBase
{
    public string nickName;
    public int DailyScore;
    public int weekScore;
    public int monScore;
    public int totalScore;
}
public class UserItem : BackEndBase
{
    public int continueCoupon = 2;
    public int shield = 2;
    public bool adsRemove;
}
public class RankingData
{
    public string gamerInDate;
    public string nickname;
    public int score;
    public int index;
    public int rank;
}
