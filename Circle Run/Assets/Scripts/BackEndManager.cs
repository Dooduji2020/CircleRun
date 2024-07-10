using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;

public class BackEndManager : MonoBehaviour
{
    private UserItem userItem = new UserItem();
    public delegate void ExceptionEvent(Exception e);
    private void Start()
    {
        var bro = Backend.Initialize(true, true);
        if(bro.IsSuccess())
        {
            //�ʱ�ȭ ����
        }
        else
        { 
            //�ʱ�ȭ ����
        }
    }
    public void GetRanking()
    {
        //�ش� ��ŷ ���̺��� 15�������� ������ �����´�
        Backend.URank.User.GetRankList("tableUUID", 50);
    }
    public T GetItemData<T>(string tableName) where T : BackEndBase, new()
    {
        var bro = Backend.GameData.GetMyData(tableName, Backend.UserInDate);
        T data = new T();
        if(bro.IsSuccess())
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
        if(bro.IsSuccess())
        { }
        else
        { }
    }
    public void GameDataUpdate(string tableName, Dictionary<string, string> dic)
    {
        Param param = new Param();
        foreach (var i in dic)
            param.Add(i.Key, i.Value);
        Backend.GameData.UpdateV2(tableName, userItem.inDate, Backend.UserInDate, param,(callback)=> { 
        });
    }
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
