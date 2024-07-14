using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingDaily : RankingGroupBase
{
    public override void Init(List<RankingData> datas)
    {
        int count = slots.Length - (slots.Length - datas.Count);
        for(int i = 0;i<slots.Length;i++)
        {
            if (count <= i)
                slots[i].Init(null);
            else
                slots[i].Init(datas[i]);
        }
    }
}
