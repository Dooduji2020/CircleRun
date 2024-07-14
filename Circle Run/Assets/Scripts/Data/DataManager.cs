using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static UserScore userScore;
    public static UserItem userItem;
    public static RankList dailyRanking = new RankList();
    public static RankList weekRanking = new RankList();

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
