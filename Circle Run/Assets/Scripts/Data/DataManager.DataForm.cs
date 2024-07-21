using System;
using System.Collections.Generic;
using BackEnd;

public class BackEndBase
{
    public string inDate;
    public string owner_inDate;
    public string updatedAt;
}
public class BackEndGameData<T> where T : BackEndBase
{
    public T[] rows;
}
#region User
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
    public int continueCoupon;
    public int shield;
    public bool adsRemove;
}
#endregion

#region Rank
public class RankingData
{
    public string gamerInDate;
    public string nickname;
    public string nickName;
    public int score;
    public int index;
    public int rank;
}
public class RankList
{
    public List<RankingData> rows = new List<RankingData>();
}
public enum Ranking
{
    Daily,
    Week
}
#endregion

#region TimeData
public class TimeData : BackEndBase
{
    public DateTime ShieldAds;
    public DateTime CouponAds;
    public DateTime Shield;
    public DateTime Coupon;
    public int ShieldAdsCount;
    public int CouponAdsCount;
}
#endregion