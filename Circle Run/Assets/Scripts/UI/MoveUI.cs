using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MoveUI : MonoBehaviour
{
    private RectTransform myRect;

    [Header("이동 방향")]
    public MoveAngle angle;

    private float moveRectSize;
    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        myRect = GetComponent<RectTransform>();
        switch (angle)
        {
            case MoveAngle.Up:
            case MoveAngle.Down:
                moveRectSize = myRect.rect.height;
                break;
            case MoveAngle.Left:
            case MoveAngle.Right:
                moveRectSize = myRect.rect.width;
                break;
        }
    }
    private void Move()
    { 
        //myRect.DOAnchorPos()
    }
}
