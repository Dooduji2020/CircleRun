using UnityEngine;
using DG.Tweening;

public class StartPlayer : MoveUI {
    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
    }

    public override void Move()
    {
        myRect.DOMoveY(0.75f, speed).SetEase(Ease.OutBack);
    }
}
