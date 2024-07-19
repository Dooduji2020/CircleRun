using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingRewardPop : MonoBehaviour
{
    public TextMeshProUGUI[] countTxt;
    public Image[] itemIMG;

    public void Open(Transform tr, int rank)
    {
        transform.position = tr.position;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
