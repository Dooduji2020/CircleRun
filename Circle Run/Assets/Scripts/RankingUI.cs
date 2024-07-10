using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    public Transform slotsTr;

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
    private void Init()
    {
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
