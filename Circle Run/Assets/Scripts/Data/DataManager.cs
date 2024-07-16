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
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public void Init()
    { }
    public void UserItemUpdate(System.Action<bool> callback)
    {
        LoadingManager.Instance.LoadingStart();
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("", userItem.continueCoupon.ToString());
        dic.Add("", userItem.shield.ToString());
        dic.Add("", userItem.adsRemove.ToString());
        BackEndManager.Instance.GameDataUpdate("UserItemData",userItem.inDate,dic, callback);
    }
}
