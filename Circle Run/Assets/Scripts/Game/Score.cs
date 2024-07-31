using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
      _destroyTime,
      _rotateSpeed;

    float _maxOffset;
    private bool hasGameFinished;
    private void Start()
    {
        hasGameFinished = false;
        ColorChanged(GameManager.Instance.CurrentColor);
        _maxOffset = GameManager.Instance.minOffsetX + -2f;
    }

    private void OnEnable()
    {
        GameManager.Instance.GameEnded += OnGameEnded;
        GameManager.Instance.ColorChanged += ColorChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameEnded -= OnGameEnded;
        GameManager.Instance.ColorChanged -= ColorChanged;
    }

    private void FixedUpdate()
    {
        if (hasGameFinished || GameManager.Instance.isPause) return;
        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.left;
        transform.Rotate(_rotateSpeed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.position.x < _maxOffset)
        {
            Destroy(gameObject);
        }
    }
    private void Move()
    {
        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.right;
        transform.Rotate(_rotateSpeed * Time.fixedDeltaTime * Vector3.forward);

        if (transform.position.x < _maxOffset)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag(Constants.Tags.BOSS))
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

        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.fixedDeltaTime;
            transform.localScale = startScale + timeElapsed * scaleOffset;
            yield return updateTime;
        }

        Destroy(gameObject);

    }

    private void ColorChanged(Color col)
    {
        GetComponent<SpriteRenderer>().color = col;
    }
}
