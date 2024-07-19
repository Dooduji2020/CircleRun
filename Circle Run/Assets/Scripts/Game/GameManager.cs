using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    #region MONOBEHAVIOUR METHODS

    public static GameManager Instance { get; private set; }

    public Player player;

    [SerializeField]
    private TMP_Text _scoreText, _endScoreText, _highScoreText;

    private int score;

    [SerializeField]
    private Animator _scoreAnimator;

    [SerializeField]
    private AnimationClip _scoreClip;

    [SerializeField]
    private GameObject _endPanel;
    [SerializeField]
    private ContinueUI _continueUI;
    [SerializeField]
    private InGameRankingUI _rankUI;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    public bool isRewardAds, isCoupon = false;

    public Image[] shieldIMG;
    public int shield;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.AddButtonSound();


        StartCoroutine(IStartGame());
    }

    #endregion

    #region UI

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.DATA.MAIN_MENU_SCENE);
    }

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleSound()
    {
        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
            : 1) == 1;
        sound = !sound;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
        AudioManager.Instance.ToggleSound();
    }

    public void UpdateScore()
    {
        score++;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        if (score % 2 == 0)
        {
            CurrentColorId++;
        }
    }

    #endregion

    #region GAME_START_END


    [SerializeField]
    private Vector3 _cameraStartPos, _cameraEndPos;
    [SerializeField]
    private float _timeToMoveCamera;

    private bool hasGameEnded;
    public UnityAction GameStarted, GameEnded;

    private IEnumerator IStartGame()
    {
        hasGameEnded = false;
        Camera.main.transform.position = _cameraStartPos;
        _scoreText.gameObject.SetActive(false);
        yield return MoveCamera(_cameraEndPos);

        //Score
        _scoreText.gameObject.SetActive(true);
        score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnObstacles2());
        StartCoroutine(SpawnBoss());


        CurrentColorId = 0;
        GameStarted?.Invoke();
    }
    private IEnumerator IReStartGame()
    {
        hasGameEnded = false;
        //Camera.main.transform.position = _cameraStartPos;
        _scoreText.gameObject.SetActive(false);
        yield return MoveCamera(_cameraEndPos);

        //Score
        _scoreText.gameObject.SetActive(true);
        //score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);

        StartCoroutine(SpawnObstacles());
        StartCoroutine(SpawnObstacles2());
        StartCoroutine(SpawnBoss());


        //CurrentColorId = 0;
        GameStarted?.Invoke();
    }

    private IEnumerator MoveCamera(Vector3 cameraPos)
    {
        Transform cameraTransform = Camera.main.transform;
        float timeElapsed = 0f;
        Vector3 startPos = cameraTransform.position;
        Vector3 offset = cameraPos - startPos;
        float speed = 1 / _timeToMoveCamera;
        while (timeElapsed < 1f)
        {
            timeElapsed += speed * Time.deltaTime;
            cameraTransform.position = startPos + timeElapsed * offset;
            yield return null;
        }
        cameraTransform.position = cameraPos;
    }
    public void ShieldUse()
    {
        --shield;
        shieldIMG[shield].gameObject.SetActive(false);
        shieldIMG[shield].DOColor(Color.gray, 1f).OnComplete(()=>shieldIMG[shield].gameObject.SetActive(false));
    }
    public void EndGame()
    {
        StartCoroutine(GameOver());
    }

    [SerializeField] private Animator _highScoreAnimator;
    [SerializeField] private AnimationClip _highScoreClip;

    private IEnumerator GameOver()
    {
        hasGameEnded = true;
        _scoreText.gameObject.SetActive(false);

        //yield return new WaitForSeconds(1f);
        if (DataManager.userItem.continueCoupon > 0 && !isCoupon)   
        {
            _continueUI.Open(false);
            isCoupon = true;
            _continueUI.closeAction += EndGame;
        }
        else if (!isRewardAds)  
        {
            _continueUI.Open(true);
            isRewardAds = true;
            _continueUI.closeAction += EndGame;
            
        }
        else
        {
            _continueUI.gameObject.SetActive(false);
            _endPanel.SetActive(true);
            yield return MoveCamera(new Vector3(_cameraStartPos.x, -_cameraStartPos.y, _cameraStartPos.z));
            _rankUI.Open(score);
        }
        _endScoreText.text = score.ToString();
        if (score > DataManager.DailyScore)
        {
            _highScoreText.text = "NEW BEST";

            //Play HighScore Animation
            _highScoreAnimator.Play(_highScoreClip.name, -1, 0f);

            DataManager.DailyScore = score;
        }
        else
        {
            _highScoreText.text = "BEST " + DataManager.DailyScore.ToString();
        }
    }
    public void GameContinuePlay()
    {
        player.GameReStart();
        StartCoroutine(IReStartDelay());
    }
    IEnumerator IReStartDelay()
    {
        yield return new WaitForSeconds(2f);
        StartCoroutine(IReStartGame());
    }

    #endregion

    #region OBSTACLE_SPAWNING

    [SerializeField]
    private List<Color> EnemyColors;

    [SerializeField]
    private GameObject _obstaclePrefab, _obstaclePrefab2, _bossPrefab, _scorePrefab;

    [SerializeField]
    private List<Vector3> _obstacleSpawnPos;
    [SerializeField]
    private List<Vector3> _obstacleSpawnPos2;
    [SerializeField]
    private List<Vector3> _bossSpawnPos;

    [SerializeField]
    private float _obstacleSpawnTime;
    [SerializeField]
    private float _obstacleSpawnTime2;
    [SerializeField]
    private float _bossSpawnTime;


    private IEnumerator SpawnObstacles()
    {
        var timeInterval = new WaitForSeconds(_obstacleSpawnTime);
        bool isScore = UnityEngine.Random.Range(0, 3) == 0;
        var spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab;
        Vector3 spawnPos = _obstacleSpawnPos[UnityEngine.Random.Range(0, _obstacleSpawnPos.Count)];

        while (!hasGameEnded)
        {
            GameObject enemy = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<SpriteRenderer>().color = GetRandomColor();
            isScore = UnityEngine.Random.Range(0, 3) == 0;
            spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab;
            spawnPos = _obstacleSpawnPos[UnityEngine.Random.Range(0, _obstacleSpawnPos.Count)];
            yield return timeInterval;

            Color GetRandomColor()
            {

                return EnemyColors[UnityEngine.Random.Range(0, EnemyColors.Count)];
            }


        }

    }

    private IEnumerator SpawnObstacles2()
    {

        var timeInterval = new WaitForSeconds(_obstacleSpawnTime2);
        bool isScore = UnityEngine.Random.Range(0, 3) == 0;
        var spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab2;
        Vector3 spawnPos = _obstacleSpawnPos2[UnityEngine.Random.Range(0, _obstacleSpawnPos2.Count)];

        while (!hasGameEnded)
        {
            GameObject enemy = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<SpriteRenderer>().color = GetRandomColor();
            isScore = UnityEngine.Random.Range(0, 3) == 0;
            spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab2;
            spawnPos = _obstacleSpawnPos2[UnityEngine.Random.Range(0, _obstacleSpawnPos2.Count)];
            yield return timeInterval;

            Color GetRandomColor()
            {

                return EnemyColors[UnityEngine.Random.Range(0, EnemyColors.Count)];
            }

        }

    }



    private IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(5.0f);
        var timeInterval = new WaitForSeconds(_bossSpawnTime);
        bool isScore = UnityEngine.Random.Range(0, 3) == 0;
        var spawnPrefab = isScore ? _scorePrefab : _bossPrefab;
        Vector3 spawnPos = _bossSpawnPos[UnityEngine.Random.Range(0, _bossSpawnPos.Count)];

        while (!hasGameEnded)
        {
            Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            isScore = UnityEngine.Random.Range(0, 3) == 0;
            spawnPrefab = isScore ? _scorePrefab : _bossPrefab;
            spawnPos = _bossSpawnPos[UnityEngine.Random.Range(0, _bossSpawnPos.Count)];
            yield return timeInterval;

        }
    }

    #endregion

    #region COLOR_CHANGE

    [SerializeField] private List<Color> _colors;

    [HideInInspector] public Color CurrentColor => _colors[CurrentColorId];

    [HideInInspector] public UnityAction<Color> ColorChanged;

    private int _currentColorId;

    private int CurrentColorId
    {
        get
        {
            return _currentColorId;
        }

        set
        {
            _currentColorId = value % _colors.Count;
            ColorChanged?.Invoke(CurrentColor);
        }
    }
    #endregion
}
