using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _moveTime, _rotateRadius;

    [SerializeField]
    private Vector3 _center;
    public CircleCollider2D circleCollider;
    private float currentRotateAngle;
    private float rotateSpeed;

    public bool canMove;
    public bool canShoot;

    private bool shield_delay = false;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;

    public SpriteRenderer shieldSpriteRenderer;

    private void Awake()
    {
        currentRotateAngle = 0f;
        canShoot = false;
        canMove = false;
        rotateSpeed = 360f / _moveTime;
    }

    private void OnEnable()
    {
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.ColorChanged += ColorChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GameStarted -= GameStarted;
        GameManager.Instance.ColorChanged -= ColorChanged;
    }

    private void GameStarted()
    {
        canMove = true;
        canShoot = true;
    }
    public void GameReStart()
    {
        this.gameObject.SetActive(true);
        GameManager.Instance.GameStarted += GameStarted;
        GameManager.Instance.ColorChanged += ColorChanged;
    }

    private void Update()
    {
        if (canShoot && Input.GetMouseButtonDown(0))
        {
            rotateSpeed *= -1f;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }

    private Vector3 direction;

    private void FixedUpdate()
    {
        if (!canMove || GameManager.Instance.isPause) return;

        currentRotateAngle += rotateSpeed * Time.fixedDeltaTime;

        direction = new Vector3(Mathf.Cos(currentRotateAngle * Mathf.Deg2Rad)
            , Mathf.Sin(currentRotateAngle * Mathf.Deg2Rad), 0);

        transform.position = _center + _rotateRadius * direction;

        if (currentRotateAngle < 0f)
        {
            currentRotateAngle = 360f;
        }
        if (currentRotateAngle > 360f)
        {
            currentRotateAngle = 0f;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            collision.gameObject.GetComponent<Score>().OnGameEnded();
        }
        if (collision.CompareTag(Constants.Tags.SCORE+"2"))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            collision.gameObject.GetComponent<Score2>().OnGameEnded();
        }

        if (collision.CompareTag(Constants.Tags.OBSTACLE) || collision.CompareTag(Constants.Tags.BOSS))
        {
            if (shield_delay)
                return;

            if (GameManager.Instance.shield > 0)
            {
                shield_delay = true;
                 Destroy(Instantiate(_explosionPrefab, collision.transform.position, Quaternion.identity), 3f);
                 Destroy(collision.gameObject);
                AudioManager.Instance.ShieldSound();
                GameManager.Instance.ShieldUse();
                StartCoroutine(ShieldDelay());
            }
            else
            {
                Destroy(Instantiate(_explosionPrefab, transform.position, Quaternion.identity), 3f);
                AudioManager.Instance.PlaySound(_loseClip);
                GameManager.Instance.EndGame();
                this.gameObject.SetActive(false);
                canMove = false;
                canShoot = false;
            }
        }

        //if(collision.CompareTag(Constants.Tags.BOSS))
        //{
        //    Destroy(Instantiate(_explosionPrefab,transform.position,Quaternion.identity), 3f);
        //    AudioManager.Instance.PlaySound(_loseClip);            
        //    GameManager.Instance.EndGame();
        //    this.gameObject.SetActive(false);
        //}
    }
    IEnumerator ShieldDelay()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        shieldSpriteRenderer.gameObject.SetActive(true);
        shieldSpriteRenderer.DOFade(0f, 1f);
        shieldSpriteRenderer.transform.DOLocalMoveY(shieldSpriteRenderer.transform.localPosition.y + 5f, 0.6f);
        shieldSpriteRenderer.transform.DOScale(1f, 1f).OnComplete(() => {
            shieldSpriteRenderer.color = Color.white;
            shieldSpriteRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            shieldSpriteRenderer.transform.localPosition = new Vector3(0,0,0);
            shieldSpriteRenderer.gameObject.SetActive(false);
        });
        renderer.DOFade(0.2f,0.2f).SetLoops(10,LoopType.Yoyo);
        GameManager.Instance.GamePause();
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.GamePlay();
        yield return new WaitForSeconds(1.5f);
        shield_delay = false;
    }

    private void ColorChanged(Color col)
    {
        GetComponent<SpriteRenderer>().color = col;
        var mm = GetComponent<ParticleSystem>().main;
        mm.startColor = col;
    }
}