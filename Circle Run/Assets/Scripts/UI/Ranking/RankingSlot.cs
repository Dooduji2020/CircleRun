using UnityEngine;
using TMPro;

public class RankingSlot : MonoBehaviour
{
    public TextMeshProUGUI nickNameTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI classTxt;

    private RankingData data;
    public void Init(RankingData _data)
    {
        if (_data == null)
        {
            data = null;
            nickNameTxt.text = "Empty";
            scoreTxt.text = "00";
            classTxt.text = "00";
        }
        else
        {
            data = _data;
            nickNameTxt.text = data.nickName;
            scoreTxt.text = data.score.ToString();
            classTxt.text = data.rank.ToString();
        }
    }
}
