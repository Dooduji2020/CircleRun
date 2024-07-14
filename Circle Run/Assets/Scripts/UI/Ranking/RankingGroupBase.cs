using System.Collections.Generic;
using UnityEngine;

public abstract class RankingGroupBase : MonoBehaviour
{
    [SerializeField]
    protected Transform slotTr;

    [SerializeField]
    protected RankingSlot[] slots;


    public abstract void Init(List<RankingData> datas);

}
