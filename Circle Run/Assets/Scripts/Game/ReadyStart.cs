using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



public class ReadyStart : MonoBehaviour {

    public Ease EaseType;

    private TextMeshProUGUI text;

    private void Start() {

        
        text = this.GetComponent<TextMeshProUGUI>();
        text.DOFade(0, 1).SetDelay(2f).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }
}
