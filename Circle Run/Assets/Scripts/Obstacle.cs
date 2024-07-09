using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
        _maxOffset,
        _destroyTime,
        _rotateSpeed;

    private bool hasGameFinished,
        canRotate,
        isVertical;

    private void Start()
    {
        hasGameFinished = false;
        canRotate = Random.Range(0, 4) == 0;
        isVertical = Random.Range(0, 2) == 0;

        transform.rotation = Quaternion.Euler(0, 0, isVertical ? 90f : 0);
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

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.left;

        if(canRotate)
        {
            transform.Rotate(_rotateSpeed * Time.fixedDeltaTime * Vector3.forward);
        }

        if (transform.position.x < _maxOffset)
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
