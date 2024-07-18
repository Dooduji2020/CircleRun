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

    private event Action sendButtonEvent;
    public Action closeAction;

    private void Awake()
    {
        sendBtn.onClick.AddListener(ContinueSend);
        sendBtn.interactable = false;
    }
    public void Open(bool isCoupon = false)
    {
        if(isCoupon)
        {
            
            BackEndManager.Instance.UseContinueCoupon((result) => {
                if (result)
                {
                    --DataManager.userItem.continueCoupon;
                    closeAction = null;
                    this.gameObject.SetActive(false);
                    //게임 진행
                }
                else
                {
                    Debug.Log("error");
                }
            });
        }
        else
        {
            sendButtonEvent += () => AdsManager.Instance.ShowRewardAd((reward) => { });
        }
        sendBtn.interactable = true;
    }
    private void ContinueSend()
    {
        sendButtonEvent?.Invoke();
        sendBtn.interactable = false;
        sendButtonEvent = null;
    }
    public void Close()
    {
        closeAction?.Invoke();
        closeAction = null;
    }
}
