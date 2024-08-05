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
    public ContinueUI _adsContinueUI;

    [SerializeField]
    private InGameRankingUI _rankUI;

    [SerializeField]
    private Image _soundImage;

    [SerializeField]
    private Sprite _activeSoundSprite, _inactiveSoundSprite;

    public bool isRewardAds, isCoupon = false;

    public Image[] shieldIMG;
    public int shield;
    public GameObject continueDelay;
    public TextMeshProUGUI continueTxt;

    [HideInInspector]
    public bool isPlay = false;
    [HideInInspector]
    public bool isPause = false;

    private float timer = 0;
    private float playTime = 0;
    private bool isBoss = false;
    public bool isAlpha = false;
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
        if (isPlay && !isPause)
        {
            timer += Time.deltaTime;
            if (timer > 1)
            {
                playTime += 1f;
                timer = 0;
                if (spwanDelay > 0.1f)
                    spwanDelay -= 0.05f;
                else if (spwanDelay == 0.1f && score > 200)
                    spwanDelay = 0f;
                score += 1;
                _scoreText.text = score.ToString();
                if (!isAlpha && score >= 15)
                    isAlpha = true;
                if (playTime % 10 == 0)
                    isBoss = true;
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
    public void GamePause()
    {
        isPause = true;
    }
    public void GamePlay()
    {
        isPause = false;
        StartCoroutine(ObstaclesSpawn());
        // StartCoroutine(SpawnObstacles());
        // StartCoroutine(SpawnObstacles2());
        // StartCoroutine(SpawnBoss());
    }
    public void TutorialStar()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 pos = new Vector3(_obstacleSpawnPos[0].x, playerPos.y, _obstacleSpawnPos[0].z);
        Score star = Instantiate(_scorePrefab, pos, Quaternion.identity).GetComponent<Score>();
        isPlay = true;
    }
    #endregion

    #region GAME_START_END


    [SerializeField]
    private Vector3 _cameraStartPos, _cameraEndPos;
    [SerializeField]
    private float _timeToMoveCamera;

    [HideInInspector]
    public float maxOffsetX, minOffsetX;


    public ShieldSelectUI selectUI;

    private bool hasGameEnded;
    private float spwanDelay = 2f;

    public UnityAction GameStarted, GameEnded;
    private void LimitedPositionX()
    {
        Camera camera = Camera.main;

        if (camera.orthographic)
        {
            float orthoSize = camera.orthographicSize;
            float aspectRatio = Screen.width / (float)Screen.height;

            minOffsetX = (camera.transform.position.x - orthoSize * aspectRatio) + -1f;
            maxOffsetX = (camera.transform.position.x + orthoSize * aspectRatio) + 1f;
        }
        int index = 0;
        foreach (var i in _obstacleSpawnPos)
        {
            i.Set(maxOffsetX, i.y, 0);
            _bossSpawnPos2[index] = i;
            ++index;
        }
        index = 0;
        foreach (var i in _obstacleSpawnPos2)
        {
            i.Set(minOffsetX, i.y, 0);
            _bossSpawnPos[index] = i;
            ++index;
        }
    }
    private IEnumerator IStartGame()
    {
        LimitedPositionX();
        //yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(1f);
        //Score
        _scoreText.gameObject.SetActive(true);
        score = 0;
        _scoreText.text = score.ToString();
        _scoreAnimator.Play(_scoreClip.name, -1, 0f);
        isPause = false;

        StartCoroutine(ObstaclesSpawn());
        //StartCoroutine(SpawnObstacles());
        //StartCoroutine(SpawnObstacles2());
        //StartCoroutine(SpawnBoss());

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

        StartCoroutine(ObstaclesSpawn());
        // StartCoroutine(SpawnObstacles());
        // StartCoroutine(SpawnObstacles2());
        // StartCoroutine(SpawnBoss());


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

    public void AdsPopOpne()
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
        continueDelay.gameObject.SetActive(true);

        float timer = 3;
        continueTxt.text = "Continue\n" + timer.ToString();
        yield return new WaitForSeconds(1.5f);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0);
            continueTxt.text = "Continue\n"+((int)timer).ToString();
            yield return null;
        }
        continueDelay.SetActive(false);
        _scoreText.gameObject.SetActive(true);
        _scoreText.text = "Start";
        yield return new WaitForSeconds(1f);
        isPause = false;
        isPlay = true;
        player.GameReStart();
        StartCoroutine(IReStartGame());
    }

    #endregion

    #region OBSTACLE_SPAWNING

    [SerializeField]
    private List<Color> EnemyColors;

    [SerializeField]
    private GameObject _obstaclePrefab, _obstaclePrefab2, _bossPrefab, _bossPrefab2, _scorePrefab, _scorePrefab2;

    [SerializeField]
    private List<Vector3> _obstacleSpawnPos;
    [SerializeField]
    private List<Vector3> _obstacleSpawnPos2;
    [SerializeField]
    private List<Vector3> _bossSpawnPos;
    [SerializeField]
    private List<Vector3> _bossSpawnPos2;

    [SerializeField]
    private float _obstacleSpawnTime;
    [SerializeField]
    private float _obstacleSpawnTime2;
    [SerializeField]
    private float _bossSpawnTime;

    private IEnumerator ObstaclesSpawn()
    {
        yield return new WaitForSeconds(_obstacleSpawnTime);
        bool isScore = UnityEngine.Random.Range(0, 3) == 0;
        bool isScore2 = UnityEngine.Random.Range(0, 3) == 0;
        var spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab;
        var spawnPrefab2 = isScore2 ? _scorePrefab2 : _obstaclePrefab2;
        int pos = UnityEngine.Random.Range(0, _obstacleSpawnPos.Count);
        int pos2 = pos == 2 ? 0 : pos++;
        Vector3 spawnPos = _obstacleSpawnPos[pos];
        Vector3 spawnPos2 = _obstacleSpawnPos2[pos2];
        while (!hasGameEnded && !isPause)
        {
            GameObject enemy = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<SpriteRenderer>().color = GetRandomColor();
            yield return new WaitForSeconds(spwanDelay);
            GameObject enemy2 = Instantiate(spawnPrefab2, spawnPos2, Quaternion.identity);
            enemy2.GetComponent<SpriteRenderer>().color = GetRandomColor();
            if (isBoss)
            {
                isBoss = false;
                bool angle = UnityEngine.Random.Range(0, 2) == 0;
                if (angle)
                {
                    pos = pos == 2 ? 0 : pos++;
                    GameObject boss = Instantiate(_bossPrefab, _bossSpawnPos[pos], Quaternion.identity);
                }
                else
                {
                    pos2 = pos2 == 2 ? 0 : pos2++;
                    GameObject boss = Instantiate(_bossPrefab2, _bossSpawnPos2[pos2], Quaternion.identity);
                }

                yield return null;
            }
            isScore = UnityEngine.Random.Range(0, 4) == 0;
            isScore2 = UnityEngine.Random.Range(0, 4) == 0;
            spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab;
            spawnPrefab2 = isScore2 ? _scorePrefab2 : _obstaclePrefab2;
            pos = UnityEngine.Random.Range(0, _obstacleSpawnPos.Count);
            pos2 = pos == 2 ? 0 : pos++;
            spawnPos = _obstacleSpawnPos[pos];
            spawnPos2 = _obstacleSpawnPos2[pos2];

            yield return new WaitForSeconds(_obstacleSpawnTime);
            Color GetRandomColor()
            {
                return EnemyColors[UnityEngine.Random.Range(0, EnemyColors.Count)];
            }
        }
    }

    private IEnumerator SpawnObstacles()
    {
        var timeInterval = new WaitForSeconds(_obstacleSpawnTime);
        bool isScore = UnityEngine.Random.Range(0, 3) == 0;
        var spawnPrefab = isScore ? _scorePrefab : _obstaclePrefab;
        Vector3 spawnPos = _obstacleSpawnPos[UnityEngine.Random.Range(0, _obstacleSpawnPos.Count)];

        while (!hasGameEnded && !isPause)
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
        var spawnPrefab = isScore ? _scorePrefab2 : _obstaclePrefab2;
        Vector3 spawnPos = _obstacleSpawnPos2[UnityEngine.Random.Range(0, _obstacleSpawnPos2.Count)];

        while (!hasGameEnded && !isPause)
        {
            GameObject enemy = Instantiate(spawnPrefab, spawnPos, Quaternion.identity);
            enemy.GetComponent<SpriteRenderer>().color = GetRandomColor();
            isScore = UnityEngine.Random.Range(0, 3) == 0;
            spawnPrefab = isScore ? _scorePrefab2 : _obstaclePrefab2;
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
        //bool isAngle = UnityEngine.Random.Range(0, 4) % 2 == 0;
        //var prefab = isAngle ? _bossPrefab : _bossPrefab2;
        var spawnPrefab = isScore ? _scorePrefab : _bossPrefab;
        //List<Vector3> pos = isAngle ? _bossSpawnPos : _obstacleSpawnPos;
        Vector3 spawnPos = _bossSpawnPos[UnityEngine.Random.Range(0, _bossSpawnPos.Count)];

        while (!hasGameEnded && !isPause)
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

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            //안내 UI 띄우기
            GamePause();
            Debug.Log("Focus Out");
        }
        else
        {
            Debug.Log("Focus On");
            if (!hasGameEnded)
                StartCoroutine(ResumeAfterDelay());
        }
    }
    private IEnumerator ResumeAfterDelay()
    {
        continueDelay.gameObject.SetActive(true);
        float timer = 3;
        continueTxt.text = timer.ToString();
        yield return new WaitForSeconds(1f);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0);
            continueTxt.text = ((int)timer).ToString();
            yield return null;
        }
        continueTxt.text = "Start!!";
        yield return new WaitForSecondsRealtime(0.5f); // 실제 시간으로 3초 대기
        continueDelay.gameObject.SetActive(false);
        GamePlay();
        Debug.Log("Time.timeScale set to 1.");
    }
}
