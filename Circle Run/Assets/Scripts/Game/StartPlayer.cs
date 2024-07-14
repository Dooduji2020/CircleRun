using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartPlayer : MonoBehaviour {

    [SerializeField]
    private float _moveTime,_rotateRadius;

    [SerializeField]
    private Vector3 _center;
    private Vector3 direction;

    private float currentRotateAngle;
    private float rotateSpeed;

    private bool canMove;
    private bool canShoot;

    /*private void Awake() {
        
        canMove = true;
        rotateSpeed = 360f / _moveTime;
    }*/

    private void Start() {
        
        Transform rt = GetComponent<Transform>();
        rt.DOMoveY(0.75f , 1.2f).SetDelay(0.4f).SetEase(Ease.OutBack);
        
    }


    /*private void FixedUpdate()
    {
        if (!canMove) return;

        currentRotateAngle += rotateSpeed * Time.fixedDeltaTime;

        direction = new Vector3(Mathf.Cos(currentRotateAngle * Mathf.Deg2Rad)
            , Mathf.Sin(currentRotateAngle * Mathf.Deg2Rad), 0);

        transform.position = _center + _rotateRadius * direction;

        if (currentRotateAngle < 0f)
        {
            currentRotateAngle = 360f;
        }
        if(currentRotateAngle > 360f)
        {
            currentRotateAngle = 0f;
        }

    }*/
}
