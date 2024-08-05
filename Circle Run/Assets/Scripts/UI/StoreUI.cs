using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class StoreUI : MonoBehaviour
{
    public StroreIAP[] iapButton;
    public TextMeshProUGUI shieldAdsTxt;
    public TextMeshProUGUI couponAdsTxt;
    public Button shieldButton;
    public Button couponButton;
    public GameObject paymentObject;

    private const string countTxt = "FREE";

    public void Init()
    {
        bool isTime = false;
        DateTime nowTime = BackEndManager.Instance.GetTime();
        if (DataManager.timeData.CouponAdsCount < 2)
        {
            DateTime couponTime = DataManager.timeData.CouponAds;
            if (nowTime.Date != couponTime.Date)
            {
                DataManager.timeData.CouponAds = nowTime;
                DataManager.timeData.CouponAdsCount = 2;
                isTime = true;
            }
        }
        if (DataManager.timeData.ShieldAdsCount < 2)
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
            BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(),DataManager.timeData.inDate);

        int shieldCount = DataManager.timeData.ShieldAdsCount;
        int couponCount = DataManager.timeData.CouponAdsCount;

        if(shieldCount == 0)
            shieldButton.interactable = false;
        if(couponCount == 0)
            couponButton.interactable = false;

        shieldAdsTxt.text = countTxt + $" {shieldCount}/2";
        couponAdsTxt.text = countTxt + $" {couponCount}/2";

        foreach (StroreIAP i in iapButton)
            i.Init();
    }
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
    private void PaymentResult(bool completed = true)
    {
        paymentObject.SetActive(true);
    }
    public void ClosePaymentObject()
    {
        paymentObject.SetActive(false);
    }
    public void RewardAds(int item)
    {
        switch (item)
        {
            case 0:  //쉴드 3개
                if (DataManager.timeData.ShieldAdsCount == 0)
                    return;
                //shieldButton.interactable = false;
                AdsManager.Instance.ShowRewardAd((reward)=> {
                    LoadingManager.Instance.LoadingStart();
                    DataManager.userItem.shield += 2;
                    BackEndManager.Instance.ItemDataUpdate(null);
                    Debug.Log("Shield Reward Ads Show");
                    int shieldCount = --DataManager.timeData.ShieldAdsCount;

                    if (DataManager.timeData.ShieldAdsCount < 2)
                        DataManager.timeData.ShieldAds = BackEndManager.Instance.GetTime();
                    if (DataManager.timeData.ShieldAdsCount > 0)
                        shieldButton.interactable = true;
                    BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    shieldAdsTxt.text = countTxt + $" {DataManager.timeData.ShieldAdsCount}/2";
                    TitleManager.Instance.ItemUISet();
                    PaymentResult();
                    LoadingManager.Instance.LoadingStop();
                });
                break;
            case 1:  // 쿠폰 2개
                if (DataManager.timeData.CouponAdsCount == 0)
                    return;
                //couponButton.interactable = false;
                AdsManager.Instance.ShowRewardAd((reward)=> {
                    LoadingManager.Instance.LoadingStart();
                    DataManager.userItem.continueCoupon += 2;
                    BackEndManager.Instance.ItemDataUpdate(null);
                    int couponCount = --DataManager.timeData.CouponAdsCount;
                    if (DataManager.timeData.CouponAdsCount < 2)
                        DataManager.timeData.CouponAds = BackEndManager.Instance.GetTime();
                    if (DataManager.timeData.CouponAdsCount > 0)
                        couponButton.interactable = true;
                    BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate);
                    couponAdsTxt.text = countTxt + $" {DataManager.timeData.CouponAdsCount}/2";
                    TitleManager.Instance.ItemUISet();
                    PaymentResult();
                    LoadingManager.Instance.LoadingStop();
                });
                break;
        }
        
    }
}
