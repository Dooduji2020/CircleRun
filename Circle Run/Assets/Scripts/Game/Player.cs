using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _moveTime,_rotateRadius;

    [SerializeField]
    private Vector3 _center;

    private float currentRotateAngle;
    private float rotateSpeed;

    private bool canMove;
    private bool canShoot;

    

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _scoreClip, _loseClip;

    [SerializeField]
    private GameObject _explosionPrefab;
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
        GameManager.Instance.ColorChanged -=ColorChanged;
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
        if(canShoot && Input.GetMouseButtonDown(0))
        {
            rotateSpeed *= -1f;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }

    private Vector3 direction;

    private void FixedUpdate()
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

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            AudioManager.Instance.PlaySound(_scoreClip);
            collision.gameObject.GetComponent<Score>().OnGameEnded();
        }

        if(collision.CompareTag(Constants.Tags.OBSTACLE) || collision.CompareTag(Constants.Tags.BOSS))
        {
            if (GameManager.Instance.shield > 0)
            {
                GameManager.Instance.ShieldUse();
                BackEndManager.Instance.UseShield();
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

    private void ColorChanged(Color col)
    {
        GetComponent<SpriteRenderer>().color = col;
        var mm = GetComponent<ParticleSystem>().main;
        mm.startColor = col;
    }
}