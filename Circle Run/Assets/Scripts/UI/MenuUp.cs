using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuUp : MonoBehaviour {

    private void Start() {
        
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOLocalMoveY(35, 1.2f).SetDelay(0.4f).SetEase(Ease.OutBack);
        
    }
    
}
