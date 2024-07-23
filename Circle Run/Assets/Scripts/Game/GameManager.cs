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
    private ContinueUI _adsContinueUI;

    [SerializeField]
    private InGameRankingUI _rankUI;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    public bool isRewardAds, isCoupon = false;

    public Image[] shieldIMG;
    public int shield;

    [HideInInspector]
    public bool isPlay = false;

    private float timer = 0;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        AudioManager.Instance.AddButtonSound();
        if (PlayerPrefs.GetInt("Tutorial", 0) > 0) StartCoroutine(IStartGame());
        else
        {
            //튜토리얼 진행
            TutorialStart();
        }
    }
    private void Update()
    {
        if (isPlay)
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                timer = 0;
                score += 1;
                _scoreText.text = score.ToString();
            }
        }
    }
    private void TutorialStart()
    {
        Instantiate(Resources.Load<Tutorial>("Prefabs/UI/Tutorial"), GameObject.Find("UICanvas").transform);
    }
    public void TutorialSpwan()
    {
        Vector3 spawnPos = _obstacleSpawnPos[UnityEngine.Random.Range(0, _obstacleSpawnPos.Count)];
        GameObject enemy = Instantiate(_obstaclePrefab, spawnPos, Quaternion.identity);
    }
    #endregion

    #region UI

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(Constants.DATA.MAIN_MENU_SCENE);
    }

    public void ReloadGame()
    {
        if (DataManager.userItem.shield > 0) selectUI.Open();
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleSound()
    {
        bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
            : 1) == 1;
        sound = !sound;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
    }

    public void UpdateScore()
    {
        score += 10;
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

    public ShieldSelectUI selectUI;

    private bool hasGameEnded;
    public UnityAction GameStarted, GameEnded;

    private IEnumerator IStartGame()
    {
        if (Define.PLAYCOUNT >= 5)
        {
            AdsManager.Instance.ShowInterstitialAd();
            Define.PLAYCOUNT = 0;
        }
        else
            ++Define.PLAYCOUNT;

        shield = DataManager.Instance.useShieldCount;
        for (int i = 0; i < shield; i++)
            shieldIMG[i].gameObject.SetActive(true);
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

        isPlay = true;
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
        //shieldIMG[shield].gameObject.SetActive(false);
        Tweener colorTween = shieldIMG[shield].DOColor(Color.gray, 1f);
        Tweener scaleTween = shieldIMG[shield].rectTransform.DOScale(new Vector3(3f, 3f, 3f), 1f);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Join(colorTween);
        mySequence.Join(scaleTween);
        mySequence.OnComplete(() => { shieldIMG[shield].rectTransform.localScale = new Vector3(1f, 1f, 1f); });
    }
    public void EndGame()
    {
        isPlay = false;
        StartCoroutine(GameOver());
    }

    [SerializeField] private Animator _highScoreAnimator;
    [SerializeField] private AnimationClip _highScoreClip;

    private void AdsPopOpne()
    {
        isRewardAds = true;
        _adsContinueUI.Open();
        _adsContinueUI.closeAction += GameOverLast;
        _continueUI.gameObject.SetActive(false);
    }
    private IEnumerator GameOver()
    {
        if (score > DataManager.DailyScore)
        {
            _highScoreText.text = "NEW BEST";

            //Play HighScore Animation
            _highScoreAnimator.Play(_highScoreClip.name, -1, 0f);

            //DataManager.DailyScore = score;
        }
        _highScoreText.text = "BEST " + (DataManager.DailyScore < score ? score.ToString() : DataManager.DailyScore.ToString());
        yield return new WaitForSeconds(1f);
        hasGameEnded = true;
        _scoreText.gameObject.SetActive(false);

        //yield return new WaitForSeconds(1f);
        if (DataManager.userItem.continueCoupon > 0 && !isCoupon)
        {
            _continueUI.Open();
            isCoupon = true;
            _continueUI.closeAction += AdsPopOpne;
        }
        else if (!isRewardAds)
        {
            _adsContinueUI.Open();
            isRewardAds = true;
            _adsContinueUI.closeAction += GameOverLast;
        }
        else
        {
            _continueUI.gameObject.SetActive(false);
            _adsContinueUI.gameObject.SetActive(false);
            _endPanel.SetActive(true);
            yield return MoveCamera(new Vector3(_cameraStartPos.x, -_cameraStartPos.y, _cameraStartPos.z));
            _rankUI.Open(score);
        }
        _endScoreText.text = score.ToString();
    }
    private void GameOverLast()
    {
        StartCoroutine(GameOverTitle());
    }
    IEnumerator GameOverTitle()
    {
        _continueUI.gameObject.SetActive(false);
        _adsContinueUI.gameObject.SetActive(false);
        _endPanel.SetActive(true);
        yield return MoveCamera(new Vector3(_cameraStartPos.x, -_cameraStartPos.y, _cameraStartPos.z));
        _rankUI.Open(score);
        _endScoreText.text = score.ToString();
    }
    public void GameContinuePlay()
    {
        StartCoroutine(IReStartDelay());
    }
    IEnumerator IReStartDelay()
    {
        yield return new WaitForSeconds(2f);
        isPlay = true;
        player.GameReStart();
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
