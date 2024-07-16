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

    private event Action sendButtonEvent;

    private void Awake()
    {
        sendBtn.onClick.AddListener(ContinueSend);
        sendBtn.interactable = false;
    }
    public void Open(bool isCoupon = false)
    {
        if(isCoupon)
        {
            //쿠폰 이미지 및 Action연결
        }
        else
        {
            sendButtonEvent += () => AdsManager.Instance.ShowRewardAd((reward) => { });
            // 광고 후 부활 이미지 
        }
        sendBtn.interactable = true;
    }
    private void ContinueSend()
    {
        sendButtonEvent?.Invoke();
        sendBtn.interactable = false;
        sendButtonEvent = null;
    }
}
