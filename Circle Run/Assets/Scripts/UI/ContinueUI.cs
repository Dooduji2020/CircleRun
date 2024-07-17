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

    private void Awake()
    {
        sendBtn.onClick.AddListener(ContinueSend);
        sendBtn.interactable = false;
    }
    public void Open(bool isCoupon = false)
    {
        if(isCoupon)
        {
            //���� �̹��� �� Action����
        }
        else
        {
            sendButtonEvent += () => AdsManager.Instance.ShowRewardAd((reward) => { });
            // ���� �� ��Ȱ �̹��� 
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
