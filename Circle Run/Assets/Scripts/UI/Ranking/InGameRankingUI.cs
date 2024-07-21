using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameRankingUI : MonoBehaviour
{
    public Sprite up;
    public Sprite noChange;

    [Header("Daily Record")]
    public Image dailyRankIMG;
    public TextMeshProUGUI dailyScore;
    public TextMeshProUGUI dailyRank;

    [Space(10)]
    [Header("Week Record")]
    public Image weekRankIMG;
    public TextMeshProUGUI weekScore;
    public TextMeshProUGUI weekRank;

    public void Open(int score)
    {
        bool isRecord = HighScore(score);
        if (!isRecord) return;

        this.gameObject.SetActive(true);
    }
    private bool HighScore(int score)
    {
        bool isScore = false;
        if (DataManager.DailyScore < score)
        {
            DataManager.DailyScore = score;
            dailyRankIMG.sprite = up;
            dailyRankIMG.color = Color.green;
            isScore = true;
        }
        else
        {
            dailyRankIMG.sprite = noChange;
            dailyRankIMG.color = Color.gray;
        }
        if (DataManager.WeekScore < score)
        {
            DataManager.WeekScore = score;
            weekRankIMG.sprite = up;
            weekRankIMG.color = Color.green;
            isScore = true;
        }
        else
        {
            weekRankIMG.sprite = noChange;
            weekRankIMG.color = Color.gray;
        }
        weekScore.text = DataManager.WeekScore.ToString();
        dailyScore.text = DataManager.DailyScore.ToString();
        if (isScore)
        {
            BackEndManager.Instance.ScoreUpdate();
            BackEndManager.Instance.GetRanking();
        }

        return isScore;
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
