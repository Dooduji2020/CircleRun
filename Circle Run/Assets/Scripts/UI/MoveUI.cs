using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveUI : MonoBehaviour
{
    private RectTransform myRect;

    [Header("이동 방향 && 시작지점 && 스피드")]
    public MoveAngle angle;
    public bool isOrigin = true;
    public float speed = 1.5f;

    private Vector2 originPos;
    private Vector2 movePos;
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        myRect = GetComponent<RectTransform>();
        originPos = myRect.anchoredPosition;
        switch (angle)
        {
            case MoveAngle.Up:
                movePos = new Vector2(0, originPos.y + myRect.rect.height);
                break;
            case MoveAngle.Down:
                movePos = new Vector2(0, originPos.y + -myRect.rect.height);
                break;
            case MoveAngle.Left:
                movePos = new Vector2(myRect.rect.width,0);
                break;
            case MoveAngle.Right:
                movePos = new Vector2(-myRect.rect.width, 0);
                break;
        }
    }
    public void Move()
    {
        if(isOrigin)
        {
            isOrigin = false;
            myRect.DOAnchorPos(movePos, speed).SetEase(Ease.OutBack);
        }
        else
        {
            isOrigin = true;
            myRect.DOAnchorPos(originPos, speed).SetEase(Ease.OutBack);
        }
    }
}
