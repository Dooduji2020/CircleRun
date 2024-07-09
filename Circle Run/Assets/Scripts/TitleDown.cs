using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleDown : MonoBehaviour {
  
  private void Start() {
    
        
        RectTransform rt = GetComponent<RectTransform>();
        rt.DOAnchorPosY(-395, 1.2f).SetDelay(0.4f).SetEase(Ease.OutBack);
        
  }
}
