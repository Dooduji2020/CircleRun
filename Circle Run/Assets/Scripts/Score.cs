using System.Collections;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed,
      _maxOffset,
      _destroyTime,
      _rotateSpeed;

    private bool hasGameFinished;

    private void Start()
    {
        hasGameFinished = false;
        ColorChanged(GameManager.Instance.CurrentColor);
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
        if (hasGameFinished) return;

        transform.position += _moveSpeed * Time.fixedDeltaTime * Vector3.left;
        transform.Rotate(_rotateSpeed * Time.fixedDeltaTime * Vector3.forward);
            
        if (transform.position.x < _maxOffset)
        {
            Destroy(gameObject);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.CompareTag(Constants.Tags.BOSS))
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
