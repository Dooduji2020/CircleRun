using System.Collections.Generic;

public class BackEndBase
{
    public string inDate;
    public string owner_inDate;
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
    public int continueCoupon = 2;
    public int shield = 2;
    public bool adsRemove = false;
}
#endregion

#region Rank
public class RankingData
{
    public string gamerInDate;
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