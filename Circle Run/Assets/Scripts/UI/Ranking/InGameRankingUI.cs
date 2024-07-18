using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameRankingUI : MonoBehaviour
{
    public bool Open(int score)
    {
        return HighScore(score);
    }
    private bool HighScore(int score)
    {
        bool isScore = false;
        if(DataManager.WeekScore < score)
            DataManager.WeekScore = score;
        if(DataManager.DailyScore < score)
            DataManager.DailyScore = score;
        this.gameObject.SetActive(isScore);
        return isScore;
    }
}
