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
        ItemTimeInit();
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
    private void ItemTimeInit()
    {
        int couponCount = userItem.continueCoupon;
        int shieldCount = userItem.shield;
        if (couponCount >= 2 && shieldCount >= 2)
            return;

        DateTime currentTime = BackEndManager.Instance.GetTime();
        if (couponCount < 2)
        {
            int index = 0;
            DateTime couponTime = timeData.Coupon;
            TimeSpan timeDifference = currentTime - couponTime;
            if (timeDifference.TotalMinutes >= 60)
                userItem.continueCoupon = 2;
            else if (timeDifference.TotalMinutes >= 30)
            {
                ++index;
                ++userItem.continueCoupon; 
            }
            if (userItem.continueCoupon < 2)
                StartCoroutine(CouponTimer(currentTime,index)); 
        }
        if (shieldCount < 2)
        {
            int index = 0;
            DateTime shieldTime = timeData.Shield;
            TimeSpan timeDifference = currentTime - shieldTime;
            if (timeDifference.TotalMinutes >= 60)
                userItem.shield = 2;
            else if (timeDifference.TotalMinutes >= 30)
            {
                ++index;
                ++userItem.shield; 
            }
            if(userItem.shield < 2)
                StartCoroutine(ShieldTimer(currentTime,index));
        }
        
    }
    IEnumerator CouponTimer(DateTime timer,int index)
    {
        int upTimer = 30;
        if (index > 0)
            upTimer += 30;
        float time = 0;
        while (userItem.continueCoupon < 2)
        {
            time += Time.deltaTime;
            if (time >= 1f)
            {
                timer = timer.AddSeconds(1);
                Debug.Log(timer);
                time = 0;
                TimeSpan timeDifference = timer - timeData.Coupon;
                if (timeDifference.TotalMinutes >= upTimer)
                {
                    ++userItem.continueCoupon;
                    upTimer += 30;
                }
            }
            yield return null;
        }
        yield break;
    }
    IEnumerator ShieldTimer(DateTime timer,int index)
    {
        int upTimer = 30;
        if (index > 0)
            upTimer += 30;
        float time = 0;
        while (userItem.shield < 2)
        {
            time += Time.deltaTime;
            if (time >= 1f)
            {
                timer = timer.AddSeconds(1);
                time = 0;
                TimeSpan timeDifference = timer - timeData.Coupon;
                if (timeDifference.TotalMinutes >= upTimer)
                {
                    ++userItem.continueCoupon;
                    upTimer += 30;
                }
            }
            yield return null;
        }
        yield break;
    }
}
