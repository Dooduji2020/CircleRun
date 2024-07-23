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
        if (!BackEndManager.isInit)
        {
            LoadingManager.Instance.LoadingStart();
            Invoke("Init", 2f);
        }
        else
        {
            ItemUISet();
            foreach (var i in moveUI)
                i.Move();
        }
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
            LoadingManager.Instance.LoadingStop();
    }
    public void VersionCheckResult(bool result)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
            Application.OpenURL("");
#elif !UNITY_EDITOR && UNITY_IOS
            Application.OpenURL("");
#endif
    }
    public void TxtInit(string nickName, string shieldCount, string couponCount)
    {
        nickNameTxt.text = nickName;
        shieldTxt.text = shieldCount;
        couponTxt.text = couponCount;
    }
    public void UserDataInit()
    {
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
                LoadingManager.Instance.LoadingStop();
                foreach (var i in moveUI)
                    i.Move();
            });
        }
        MainMenuManager.Instance.Init();
        yield break;
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
