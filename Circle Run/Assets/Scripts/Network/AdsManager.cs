using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    //보상광고
    private const string rewardAdID = "ca-app-pub-7346251641612501/6566400313";
    private const string testRewardAdID = "ca-app-pub-3940256099942544/5224354917";

    //전면광고
    private const string InterstitialAdID = "ca-app-pub-7346251641612501/6597232048";
    private const string testInterstitialAdID = "ca-app-pub-3940256099942544/1033173712";

    public bool testMode = true;

    private InitializationStatus initStatus;
    private InterstitialAd _interstitialAd;
    private RewardedAd _rewardAd;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public void Init()
    {
        RequestConfiguration requestConfiguration = new RequestConfiguration();
        requestConfiguration.TagForChildDirectedTreatment = TagForChildDirectedTreatment.True;
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize((InitializationStatus _initStatus) =>
        {
            initStatus = _initStatus;
            //InterstitialAdLoad();
            RewardAdLoad();
        });
    }
    private void AdsInitCheck()
    {
        IDictionary<string, AdapterStatus> adapterStatusMap = initStatus.getAdapterStatusMap();
        foreach (KeyValuePair<string, AdapterStatus> pair in adapterStatusMap)
        {
            string className = pair.Key;
            AdapterStatus status = pair.Value;
            switch (status.InitializationState)
            {
                case AdapterState.NotReady:
                    Debug.LogError("Adapter: " + className + " not ready. Description: " + status.Description);
                    break;
                case AdapterState.Ready:
                    Debug.Log("Adapter: " + className + " is initialized.");
                    break;
            }
        }
        if (initStatus != null && adapterStatusMap.Count > 0)
        {
            Debug.Log("Google Mobile Ads SDK initialized successfully.");
            // 초기화 성공 시 광고 로드 등의 추가 작업을 수행할 수 있습니다.
        }
        else
        {
            Debug.LogError("Google Mobile Ads SDK initialization failed.");
            // 초기화 실패 시 오류 처리 작업을 수행할 수 있습니다.
        }
    }
    private void InterstitialAdLoad()
    {
        AdRequest adRequest = new AdRequest();
        InterstitialAd.Load(testMode ? testInterstitialAdID : InterstitialAdID, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
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
        RewardedAd.Load(testMode ? testRewardAdID : rewardAdID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.Log("Ads Load Error = " + error.GetMessage());
            }
            _rewardAd = ad;
        });
    }
    public void ShowRewardAd(System.Action<Reward> callback)
    {
        if (_rewardAd == null || !_rewardAd.CanShowAd())
        {
            return; 
        }
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
