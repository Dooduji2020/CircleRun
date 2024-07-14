using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUI : MonoBehaviour
{
    public Toggle[] toggles;
    public RankingToggle[] _toggles;
    public GameObject weekObj;
    public GameObject dailyObj;
    [ContextMenu("Setting")]
    private void Setting()
    {
    }
    private void Awake()
    {
        for (int i = 0; i <= (int)Ranking.Week; i++)
        {
            int index = i;
            toggles[index].onValueChanged.AddListener((isOn) => {
                if (isOn)
                    Init((Ranking)index);
                _toggles[index].OnToggle(isOn);
                Debug.Log(index);
            });
        }
        toggles[0].isOn = true;
    }
    private void RankingSlotInit(RankList data)
    {
        // int count = rankingSlots.Length - (rankingSlots.Length - data.rows.Count);
        // for(int i = 0;i<rankingSlots.Length;i++)
        // {
        //     if (count <= i)
        //         rankingSlots[i].Init(null);
        //     else
        //         rankingSlots[i].Init(data.rows[i]);
        // }
    }
    private void Init(Ranking ranking = Ranking.Daily)
    {
        RankList data = new RankList();
        switch(ranking)
        {
            case Ranking.Daily:
                data = DataManager.dailyRanking;
                break;
            case Ranking.Week:
                data = DataManager.weekRanking;
                break;
        }
        RankingSlotInit(data);
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
