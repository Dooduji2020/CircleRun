using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                    closeAction = null;
                    GameManager.Instance.GameContinuePlay();
                    this.gameObject.SetActive(false);
                    //게임 진행
                }
                else
                {
                    Debug.Log("error");
                }
            });
        };
        }
        else
        {
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
