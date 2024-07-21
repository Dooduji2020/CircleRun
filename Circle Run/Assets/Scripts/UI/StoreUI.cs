using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class StoreUI : MonoBehaviour
{
    public TextMeshProUGUI shieldAdsTxt;
    public TextMeshProUGUI couponAdsTxt;
    public Button shieldButton;
    public Button couponButton;

    private const string countTxt = "FREE";


    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        bool isTime = false;
        DateTime nowTime = BackEndManager.Instance.GetTime();
        if (DataManager.timeData.CouponAdsCount == 2)
        {
            DateTime couponTime = DataManager.timeData.CouponAds;
            if (nowTime.Date != couponTime.Date)
            {
                DataManager.timeData.CouponAds = nowTime;
                DataManager.timeData.CouponAdsCount = 2;
                isTime = true;
            }
        }
        if (DataManager.timeData.ShieldAdsCount == 2)
        {
            DateTime shieldTime = DataManager.timeData.ShieldAds;
            if (nowTime.Date != shieldTime.Date)
            {
                DataManager.timeData.ShieldAds = nowTime;
                DataManager.timeData.ShieldAdsCount = 2;
                isTime = true;
            }
        }
        if(isTime)
            BackEndManager.Instance.SendQueueTimeUpdate(DataManager.Instance.GetTimeParam(),DataManager.timeData.inDate);

        int shieldCount = DataManager.timeData.ShieldAdsCount;
        int couponCount = DataManager.timeData.CouponAdsCount;

        shieldAdsTxt.text = countTxt + $" {shieldCount}/2";
        couponAdsTxt.text = countTxt + $" {couponCount}/2";
    }
    public void RewardAds(int item)
    {
        switch (item)
        {
            case 0:  //쉴드 3개
                if (DataManager.timeData.ShieldAdsCount == 0)
                    return;
                shieldButton.interactable = false;
                AdsManager.Instance.ShowRewardAd((reward) =>
                {
                    DataManager.userItem.shield += 3;
                    Debug.Log("Shield Reward Ads Show");
                    int shieldCount = --DataManager.timeData.ShieldAdsCount;
                    BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    shieldButton.interactable = true;
                    shieldAdsTxt.text = countTxt + $" {shieldCount}/2";
                    BackEndManager.Instance.ItemDataUpdate(null);
                    if (DataManager.timeData.ShieldAdsCount == 0)
                    {
                        DataManager.timeData.ShieldAds = BackEndManager.Instance.GetTime();
                        BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    }
                    TitleManager.Instance.ItemUISet();
                });
                break;
            case 1:  // 쿠폰 2개
                if (DataManager.timeData.CouponAdsCount == 0)
                    return;
                couponButton.interactable = false;
                AdsManager.Instance.ShowRewardAd((reward) =>
                {
                    DataManager.userItem.continueCoupon += 2;
                    int couponCount = --DataManager.timeData.CouponAdsCount;
                    BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    shieldButton.interactable = true;
                    shieldAdsTxt.text = countTxt + $" {couponCount}/2";
                    BackEndManager.Instance.ItemDataUpdate(null);
                    if (DataManager.timeData.CouponAdsCount == 0)
                    {
                        DataManager.timeData.CouponAds = BackEndManager.Instance.GetTime();
                        BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    }
                    TitleManager.Instance.ItemUISet();
                });

                break;
        }
        
    }
}
