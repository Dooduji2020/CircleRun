using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUI : MonoBehaviour
{
    public Transform slotsTr;
    public Toggle[] toggles;
    public GameObject[] rainkingPanelObject;
    private RankingSlot[] rankingSlots;
    [ContextMenu("Setting")]
    private void Setting()
    {
        rankingSlots = new RankingSlot[slotsTr.childCount];
        int index = 0;
        foreach(Transform i in slotsTr)
        {
            RankingSlot slot = i.GetComponent<RankingSlot>();
            rankingSlots[index] = slot;
        }
    }
    private void Awake()
    {
        for (int i = 0; i <= (int)Ranking.Total; i++)
        {
            toggles[i].onValueChanged.AddListener((isOn) => {
                if (isOn)
                {
                    rainkingPanelObject[i].SetActive(true);
                    Init((Ranking)i);
                }
                else
                    rainkingPanelObject[i].SetActive(false);
            });
        }
    }
    private void RankingSlotInit(List<DataManager.RankingData> data)
    {
    }
    private void Init(Ranking ranking = Ranking.Daily)
    {
        List<DataManager.RankingData> data = new List<DataManager.RankingData>();
        switch(ranking)
        {
            case Ranking.Daily:
                data = DataManager.dailyRanking;
                break;
            case Ranking.Week:
                data = DataManager.weekRanking;
                break;
            case Ranking.Mon:
                data = DataManager.monRanking;
                break;
            case Ranking.Total:
                data = DataManager.totalRanking;
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
