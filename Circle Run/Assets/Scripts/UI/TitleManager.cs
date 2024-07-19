using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using TMPro;
public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    public TextMeshProUGUI nickNameTxt;
    public TextMeshProUGUI shieldTxt;
    public TextMeshProUGUI couponTxt;
    public NickNameUI nickNameUI;

    [Header("Move UI")]
    public MoveUI[] moveUI;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject); 
    }
    private void Start()
    {
        LoadingManager.Instance.LoadingStart();
        Invoke("Init", 2f);
    }
    private void Init()
    {
        if (!BackEndManager.isInit)
            VersionCheckResult(BackEndManager.Instance.Init());
        else
            LoadingManager.Instance.LoadingStop();
    }
    private void VersionCheckResult(bool result)
    {
        if (result)
        {
            BackEndManager.Instance.GetRanking();
            StartCoroutine(LogoDelay());
        }
        else
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            Application.OpenURL("");
#elif !UNITY_EDITOR && UNITY_IOS
            Application.OpenURL("");
#endif
        }
    }
    public void TxtInit(string nickName, string shieldCount, string couponCount)
    {
        nickNameTxt.text = nickName;
        shieldTxt.text = shieldCount;
        couponTxt.text = couponCount;
    }
    public void UserDataInit()
    {
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
            Debug.Log(DataManager.userScore.DailyScore);
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
                LoadingManager.Instance.LoadingStop();
                foreach (var i in moveUI)
                    i.Move();
            });
        }
        yield break;
    }
}
