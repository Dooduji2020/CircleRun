using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class RankingGroupBase : MonoBehaviour
{
    [SerializeField]
    protected Transform slotTr;

    [SerializeField]
    protected List<RankingSlot> slots;

    [SerializeField]
    protected Button button;

    public abstract void Init(List<RankingData> datas);

}
