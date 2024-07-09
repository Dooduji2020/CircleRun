using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : MonoBehaviour {

  
    [SerializeField]
    private float _moveSpeed, _maxOffset, _destroyTime;
        

    private bool hasGameFinished,
        
        isVertical;

    private void Start()
    {
        hasGameFinished = false;
        
        isVertical = Random.Range(0, 2) == 0;
        transform.rotation = Quaternion.Euler(0, 0, -90f);
        
    }

    private void OnEnable()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
    }

    private void FixedUpdate()
    {
        if (hasGameFinished) return;

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.right;

        if (transform.position.x > _maxOffset)
        {
            Destroy(gameObject);
        }
    }

    public void OnGameEnded()
    {
        GetComponent<Collider2D>().enabled = false;
        hasGameFinished = true;
        StartCoroutine(Rescale());
    }

    private IEnumerator Rescale()
    {
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        Vector3 scaleOffset = endScale - startScale;
        float timeElapsed = 0f;
        float speed = 1 / _destroyTime;
        var updateTime = new WaitForFixedUpdate();

        while(timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.localScale = startScale + timeElapsed * scaleOffset;
            yield return updateTime;
        }

        Destroy(gameObject);

    }
}
