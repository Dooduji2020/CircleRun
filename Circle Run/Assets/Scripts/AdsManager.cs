using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    //º¸»ó±¤°í
    private const string rewardAdID = "ca-app-pub-7346251641612501/6566400313";
    private const string testRewardAdID = "ca-app-pub-3940256099942544/5224354917";

    //Àü¸é±¤°í
    private const string InterstitialAdID = "ca-app-pub-7346251641612501/6597232048";
    private const string testInterstitialAdID = "ca-app-pub-3940256099942544/1033173712";

    public bool testMode = true;

    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardAd;
    private void Init()
    {
        RequestConfiguration requestConfiguration = new RequestConfiguration();
        requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            InterstitialAdLoad();
            RewardAdLoad();
        });
    }
    private void InterstitialAdLoad()
    {
        AdRequest adRequest = new AdRequest();
        InterstitialAd.Load(testMode ? testInterstitialAdID : InterstitialAdID, adRequest, (InterstitialAd ad, LoadAdError error) => {
            if (error != null || ad == null)
            {
                Debug.Log("Ads Error");
                return;
            }
            _interstitialAd = ad;
        });
    }
    private void RewardAdLoad()
    {
        AdRequest adRequest = new AdRequest();
        RewardedAd.Load(testMode ? testRewardAdID : rewardAdID, adRequest, (RewardedAd ad, LoadAdError error) => { 
            if(error != null || ad == null)
            {
                Debug.Log("Ads Load Error = " + error.GetMessage());
            }
            _rewardAd = ad;
        });
    }
    public void ShowRewardAd(System.Action<Reward> callback)
    {
        if (_rewardAd == null || !_rewardAd.CanShowAd())
            return;
        
        _rewardAd.Show(callback);
        RewardAdLoad();
    }
    public void ShowInterstitialAd()
    {
        if (_interstitialAd == null || !_interstitialAd.CanShowAd())
            return;

        _interstitialAd.Show();
        InterstitialAdLoad();
    }
}
