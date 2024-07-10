using UnityEngine;
using TMPro;

public class RankingSlot : MonoBehaviour
{
    public TextMeshProUGUI nickNameTxt;
    public TextMeshProUGUI scoreTxt;

    private RankingData data;
    public void Init(RankingData _data)
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
            nickNameTxt.text = data.nickname;
            scoreTxt.text = data.score.ToString();
        }
    }
}
