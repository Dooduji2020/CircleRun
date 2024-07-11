using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static UserScore userScore;
    public static UserItem userItem;
    public static List<RankingData> dailyRanking = new List<RankingData>();
    public static List<RankingData> weekRanking = new List<RankingData>();
    public static List<RankingData> monRanking = new List<RankingData>();
    public static List<RankingData> totalRanking = new List<RankingData>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            DontDestroyOnLoad(gameObject);
    }
    public void Init()
    { }
}
