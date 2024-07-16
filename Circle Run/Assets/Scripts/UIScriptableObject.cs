using UnityEngine;

[CreateAssetMenu]
public class UIScriptableObject : ScriptableObject
{
    public Sprite[] uiSprite;

    public Sprite GetSprite(int index) => uiSprite[index];
}
