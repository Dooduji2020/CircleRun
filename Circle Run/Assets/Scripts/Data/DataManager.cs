using System;
using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public static UserScore userScore;
    public static UserItem userItem;
    public static RankList dailyRanking;
    public static RankList weekRanking;
    public static TimeData timeData;

    public int useShieldCount = 0;

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
        timeData = BackEndManager.Instance.GetGameData<TimeData>("TimeCheck").rows[0];

        callback?.Invoke();
    }
    public Param GetTimeParam()
    {
        Debug.LogError(timeData.Coupon.ToString("yyyy-MM-dd HH:mm:ss"));
        Param param = new Param();
        param.Add("Coupon",timeData.Coupon.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("Shield",timeData.Shield.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("CouponAds",timeData.CouponAds.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("ShieldAds",timeData.ShieldAds.ToString("yyyy-MM-dd HH:mm:ss"));
        param.Add("ShieldAdsCount",timeData.ShieldAdsCount);
        param.Add("CouponAdsCount",timeData.CouponAdsCount);
        return param;
    }
}
