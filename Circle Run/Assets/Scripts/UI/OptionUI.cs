using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Events;

public class OptionUI : MonoBehaviour
{
    public static OptionUI Instance { get; private set; }
    public TextMeshProUGUI nickNameTxt;
    public GameObject[] soundObj;

    public Button googleLoginButton;
    public Button appleLoginButton;
    public Button logOut;
    public Button deleteUser;
    public Button restoreButton;
    [Header("토글")]
    public Toggle[] languageToggles;
    public Image[] togglesIMG;
    public TextMeshProUGUI[] togglesTxt;
    [Header("로그인 안내")]
    public GameObject loginInfoPop;
    public Button loginSend;
    public Button loginCancel;

    private Color originColor = new Color(1, 0.9764f, 0.8156f, 1);
    private Color originTxtColor = new Color(0.8490566f, 0.484603f, 0.484603f, 1);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            this.gameObject.SetActive(false);
            loginCancel.onClick.AddListener(()=>loginInfoPop.gameObject.SetActive(false));
        }
        else Destroy(this.gameObject);
    }
    public void Init()
    {
        nickNameTxt.text = BackEndManager.myNickName;
        int index = PlayerPrefs.GetInt("Sound", 1);
        SoundOnOff(index);
        for (int i = 0; i < languageToggles.Length; i++)
        {
            int j = i;
            languageToggles[j].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    LocalizationManager.Instance.language = (Language)j;
                    togglesIMG[j].color = Color.white;
                    //togglesTxt[j].color = Color.black;
                }
                else
                {
                    togglesIMG[j].color = originColor;
                    //togglesTxt[j].color = originTxtColor;
                }
            });
        }
        //logOut.onClick.AddListener(()=>{});
        deleteUser.onClick.AddListener(UserDeleted);
    }
    public void LoginCheck(bool isLogin)
    {
        if(isLogin)
        {
            appleLoginButton.gameObject.SetActive(false);
            //logOut.gameObject.SetActive(true);
            deleteUser.gameObject.SetActive(true);
            nickNameTxt.text = BackEndManager.myNickName;
        }
        else
        {
            appleLoginButton.gameObject.SetActive(true);
            //logOut.gameObject.SetActive(false);
            deleteUser.gameObject.SetActive(false);
        }
        loginInfoPop.SetActive(false);
    }
    private void LogOut()
    {
        BackEndManager.Instance.Logout();
    }
    private void UserDeleted()
    {
        LoadingManager.Instance.LoadingStart();
        BackEndManager.Instance.UserDeleted();
    }
    private void Restore()
    {
        LoadingManager.Instance.LoadingStart();
        DataManager.Instance.Init(()=>{
            LoadingManager.Instance.LoadingStop();
        });
    }
    public void Open()
    {
        this.gameObject.SetActive(true);
        int index = LocalizationManager.Instance.GetLanguage();
        languageToggles[index].isOn = true;
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
    public void SoundOnOff(int mute)
    {
        if (mute == 0)
        {
            soundObj[mute].SetActive(false);
            soundObj[mute + 1].SetActive(true);
        }
        else
        {
            soundObj[mute].SetActive(false);
            soundObj[mute - 1].SetActive(true);
        }
        PlayerPrefs.SetInt("Sound", mute);
        bool isMute = mute > 0 ? false : true;
        AudioManager.Instance.SoundMute(isMute);
    }
    public void OpenLoginInfo(UnityAction action)
    {
        loginSend.onClick.RemoveAllListeners();
        loginInfoPop.gameObject.SetActive(true);
        loginSend.onClick.AddListener(action);
        LoadingManager.Instance.LoadingStop();
    }
}
