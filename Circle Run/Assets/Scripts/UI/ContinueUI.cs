using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ContinueUI : MonoBehaviour
{
    public Image IMG;
    public TextMeshProUGUI continueTxt;
    public Button sendBtn;
    public MoveUI moveUI;
    public bool isCoupon = true;
    private event Action sendButtonEvent;
    public Action closeAction;
    private void Awake()
    {
        sendBtn.onClick.AddListener(ContinueSend);
    }
    private void Start()
    {
        moveUI.Move();
    }
    public void Open()
    {
        if (isCoupon)
        {
            sendButtonEvent += () =>
        {
            BackEndManager.Instance.UseContinueCoupon((result) =>
            {
                if (result)
                {
                    --DataManager.userItem.continueCoupon;
                    if (DataManager.userItem.continueCoupon == 0)
                    {
                        DataManager.timeData.Coupon = BackEndManager.Instance.GetTime();
                        BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate, () =>
                        {
                            closeAction = null;
                            GameManager.Instance.isRewardAds = false;
                            GameManager.Instance._adsContinueUI.gameObject.SetActive(false);
                            GameManager.Instance.GameContinuePlay();
                            this.gameObject.SetActive(false);
                        });
                    }
                    else
                    {
                        closeAction = null;
                        GameManager.Instance.isRewardAds = false;
                        GameManager.Instance._adsContinueUI.gameObject.SetActive(false);
                        GameManager.Instance.GameContinuePlay();
                        this.gameObject.SetActive(false);
                    }
                    //게임 진행
                }
                else
                {
                    Debug.Log("error");
                    if (!NetworkConnect.CheckConnectInternet())
                    {
                        if (NetworkConnect.tryCount >= 3)
                        {
                            NetErrorUI errorUI = Instantiate(Resources.Load<NetErrorUI>("Prefabs/UI/NetWorkErrorUI"));
                            errorUI.confirmTxt.SetActive(true);
                            errorUI.retryTxt.SetActive(false);
                            errorUI.errorInfoTxt.SetActive(false);
                            errorUI.gameResetTxt.SetActive(true);
                            errorUI.Init(() =>
                            {
                                GameManager.Instance.GoToMainMenu();
                            });
                        }
                        else
                        {
                            InfoUI infoUI = Instantiate(Resources.Load<InfoUI>("Prefabs/UI/InfoUI"));
                            infoUI.Open("Network_Error");
                            sendBtn.interactable = true;
                        }
                    }
                    else
                    {
                        InfoUI infoUI = Instantiate(Resources.Load<InfoUI>("Prefabs/UI/InfoUI"));
                        infoUI.Open("Network_Error");
                        sendBtn.interactable = true;
                    }
                    GameManager.Instance.isRewardAds = false;
                    GameManager.Instance._adsContinueUI.gameObject.SetActive(false);
                }
            });
        };
        }
        else
        {
            if (sendButtonEvent != null)
                sendButtonEvent = null;
                
            sendButtonEvent += () => AdsManager.Instance.ShowRewardAd((reward) =>
            {
                closeAction = null;

                GameManager.Instance.GameContinuePlay();
                this.gameObject.SetActive(false);
            });
        }
        sendBtn.interactable = true;
        this.gameObject.SetActive(true);
    }
    private void ContinueSend()
    {
        if (isCoupon)
            GameManager.Instance.AdsPopOpne();
        sendBtn.interactable = false;
        sendButtonEvent?.Invoke();
        sendButtonEvent = null;
        closeAction = null;
    }
    public void Close()
    {
        closeAction?.Invoke();
        closeAction = null;
        sendButtonEvent = null;
    }
}
