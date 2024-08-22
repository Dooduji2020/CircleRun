using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    public TextMeshProUGUI nickNameTxt;
    public TextMeshProUGUI shieldTxt;
    public TextMeshProUGUI couponTxt;
    public NickNameUI nickNameUI;
    public GameObject loginErrorObj;
    public GameObject rewardPop;
    public TextMeshProUGUI shield;
    public TextMeshProUGUI coupon;
    public GameObject idlePanel;
    [Header("Move UI")]
    public MoveUI[] moveUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        if (!BackEndManager.isInit)
        {
            LoadingManager.Instance.LoadingStart();
            Invoke("Init", 2f);
        }
        else
        {
            Init();
            ItemUISet();
            foreach (var i in moveUI)
                i.Move();
        }
    }
    public void Idle()
    {
        idlePanel.gameObject.SetActive(true);
        LoadingManager.Instance.LoadingStop();
    }
    public void ItemUISet()
    {
        nickNameTxt.text = Backend.UserNickName;
        shieldTxt.text = DataManager.userItem.shield.ToString();
        couponTxt.text = DataManager.userItem.continueCoupon.ToString();
    }
    private void Init()
    {
        if (!BackEndManager.isInit)
        {
            BackEndManager.Instance.Init();

        }
        else
        {
            //if (!NetworkConnect.CheckConnectInternet())
            //{ }
            //else {
            MainMenuManager.Instance.Init();
            LoadingManager.Instance.LoadingStop();
            //}
        }
    }
    public void VersionCheckResult(bool result)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.novembernine.dongrami");
#elif !UNITY_EDITOR && UNITY_IOS
            Application.OpenURL("https://apps.apple.com/app/dongrami-dodge-the-blocks/id6566185602");
#endif
    }
    public void TxtInit(string nickName, string shieldCount, string couponCount)
    {
        nickNameTxt.text = nickName;
        shieldTxt.text = shieldCount;
        couponTxt.text = couponCount;
    }
    public void TimerText(int index, int min, int sec)
    {
        string time = $"{min}:{sec}";
        if (index == 0) shieldTxt.text = time;
        else couponTxt.text = time;
    }
    public void UserDataInit()
    {
        idlePanel.SetActive(false);
        Debug.Log("User Data Init");
        BackEndManager.Instance.GetRanking();
        StartCoroutine(LogoDelay());
    }
    IEnumerator LogoDelay()
    {
        yield return new WaitForSeconds(1.5f);
        float timer = 0;
        while (SendQueue.UnprocessedFuncCount > 0 && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        if (timer >= 5f && SendQueue.UnprocessedFuncCount > 0)
        {
            //Backend SendQueue 작업 실패 시 (유저 데이터 초기화 및 데이터 로드)
            //Debug.Log(DataManager.userScore.DailyScore);
        }
        else
        {
            if (Backend.UserNickName.Length > 2)
                nickNameTxt.text = Backend.UserNickName;
            else
                nickNameUI.gameObject.SetActive(true);

            DataManager.Instance.Init(() =>
            {
                couponTxt.text = DataManager.userItem.continueCoupon.ToString();
                shieldTxt.text = DataManager.userItem.shield.ToString();
                BackEndManager.Instance.GetRankReward();

                foreach (var i in moveUI)
                    i.Move();

                ItemUISet();
            });
        }
        AdsManager.Instance.Init();
        //NetworkManager.Instance.IAPInit();
        MainMenuManager.Instance.Init();
        yield return new WaitForSeconds(1.5f);
        yield break;
    }
    public void RankRewardPop(int _shield, int _coupon)
    {
        shield.text = _shield.ToString();
        coupon.text = _coupon.ToString();
        rewardPop.SetActive(true);
    }
    public void RankRewardClose()
    {
        rewardPop.SetActive(false);
    }
    public void LoginError()
    {
        loginErrorObj.SetActive(true);
        LoadingManager.Instance.LoadingStop();
    }
    public void GameReStart()
    {
        SceneManager.LoadScene(0);
    }

}
