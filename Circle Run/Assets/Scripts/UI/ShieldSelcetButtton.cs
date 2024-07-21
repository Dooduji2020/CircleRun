using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldSelcetButtton : MonoBehaviour
{
    public ShieldSelectUI selectUI;

    private Image IMG;
    private Button myBtn;
    private bool isUse = false;

    public void Init()
    {
        if (IMG == null)
        {
            IMG = GetComponent<Image>();
            myBtn = GetComponent<Button>();
            myBtn.onClick.AddListener(OnOffButton);
        }
        isUse = false;
        IMG.color = Color.gray;
    }
    private void OnOffButton()
    {
        if (isUse)
        {
            IMG.color = Color.gray;
            --selectUI.shieldCount;
        }
        else
        {
            if (DataManager.userItem.shield <= selectUI.shieldCount)
                return;
            ++selectUI.shieldCount;
            IMG.color = Color.white;
        }
        Debug.Log("use shield count = " + selectUI.shieldCount);
        isUse = !isUse;
    }
}
