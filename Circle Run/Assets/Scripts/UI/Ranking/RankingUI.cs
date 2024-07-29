using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class RankingUI : MonoBehaviour
{
    public Toggle[] toggles;
    public RankingToggle[] _toggles;
    public GameObject[] rankObj;
    public RankingDaily dailyRank;
    public RankingWeek weekRank;
    public GameObject rewardPop;
    public GameObject rewardParent;
    public TextMeshProUGUI[] itemCount;

    [ContextMenu("Setting")]
    private void Setting()
    {
    }
    private void Awake()
    {
        for (int i = 0; i <= (int)Ranking.Week; i++)
        {
            int index = i;
            Init((Ranking)index);
            toggles[index].onValueChanged.AddListener((isOn) => {
                rankObj[index].SetActive(isOn);
                _toggles[index].OnToggle(isOn);
                Debug.Log(index);
            });
        }
        toggles[0].isOn = true;
    }
    private void Init(Ranking ranking = Ranking.Daily)
    {
        RankList data = new RankList();
        switch(ranking)
        {
            case Ranking.Daily:
                data = DataManager.dailyRanking;
                dailyRank.Init(data.rows);
                break;
            case Ranking.Week:
                data = DataManager.weekRanking;
                weekRank.Init(data.rows);
                break;
        }
    }
    public void OpenRewardPop(int index)
    {
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        switch(index)
        {
            case 1:
                itemCount[0].text = "2";
                itemCount[1].text = "2";
                break;
            case 2:
                itemCount[0].text = "1";
                itemCount[1].text = "1";
                break;
            case 3:
                itemCount[0].text = "1";
                itemCount[1].text = "0";
                break;
        }
        rewardPop.transform.position = selectedButton.transform.position;
        rewardParent.SetActive(true);
    }
    public void CloseRewardPop()
    {
        rewardParent.SetActive(false);
    }
    public void Open()
    {
        Init();
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
