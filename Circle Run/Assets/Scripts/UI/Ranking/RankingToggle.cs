using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingToggle : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image IMG;
    private Color color = new Color(1.0f, 0.6705882f, 0.3921569f);
    public void OnToggle(bool isOn)
    {
        if (isOn)
        { 
            text.color = Color.black;
            IMG.color = Color.white;
        }
        else
        {
            text.color = Color.white;
            IMG.color = color;
        }
    }
}
