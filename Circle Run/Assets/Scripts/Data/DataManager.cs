using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public static UserScore userScore;
    public static UserItem userItem;
    public static RankList dailyRanking;
    public static RankList weekRanking;

    public static int DailyScore
    {
        set
        {
            if (userScore.DailyScore < value)
            {
                userScore.DailyScore = value;
                //BackEndManager.Instance.RankingUpdate(Ranking.Daily, value);
            }
        }
        get => userScore.DailyScore;
    }
    public static int WeekScore
    {
        get => userScore.weekScore;
        set
        {
            if (userScore.weekScore < value)
            {
                userScore.weekScore = value;
                //BackEndManager.Instance.RankingUpdate(Ranking.Week, value);
            }
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public void Init(Action callback)
    { 
        userItem = BackEndManager.Instance.GetGameData<UserItem>("UserItemData").rows[0];
        userScore = BackEndManager.Instance.GetGameData<UserScore>("UserData").rows[0];

        callback?.Invoke();
    }

}
