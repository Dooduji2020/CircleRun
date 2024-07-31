using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingWeek : RankingGroupBase
{
    public override void Init(List<RankingData> datas)
    {
        int index = 0;
        foreach(var i in datas)
        {
            slots[index].Init(i);
            ++index;
        }
    }
    public override void OnOff(bool isOn)
    {
        this.gameObject.SetActive(isOn);
    }
}
