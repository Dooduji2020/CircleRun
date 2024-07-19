using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingDaily : RankingGroupBase
{
    public override void Init(List<RankingData> datas)
    {
        for (int i = 0; i < datas.Count; i++)
        {
            if (i >= slots.Count)
            {
                RankingSlot slot = Instantiate(Resources.Load<RankingSlot>("Prefabs/UI/normal_score"),slotTr);
                slot.Init(datas[i]);
                slots.Add(slot);
            }
            else
                slots[i].Init(datas[i]);
        }
    }
}
