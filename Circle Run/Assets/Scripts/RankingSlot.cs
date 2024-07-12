using UnityEngine;
using TMPro;

public class RankingSlot : MonoBehaviour
{
    public TextMeshProUGUI nickNameTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI classTxt;

    private DataManager.RankingData data;
    public void Init(DataManager.RankingData _data)
    {
        if (_data == null)
        {
            data = null;
            nickNameTxt.text = "Empty";
            scoreTxt.text = "Empty";
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
