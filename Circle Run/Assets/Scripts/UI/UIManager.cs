using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private UIScriptableObject spriteScriptable;

    [Header("UI ÆË¾÷")]
    public ContinueUI continueUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            spriteScriptable = Resources.Load<UIScriptableObject>("UISpriteTable");
        }
        else
            Destroy(this.gameObject);
    }

    public Sprite GetSprite(UISprite sprite) => spriteScriptable.GetSprite((int)sprite);

}
