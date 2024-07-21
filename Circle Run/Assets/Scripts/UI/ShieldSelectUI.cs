using UnityEngine;
using UnityEngine.SceneManagement;
public class ShieldSelectUI : MonoBehaviour
{
    private int _shieldCount = 0;
    public int shieldCount
    {
        get => _shieldCount;
        set
        {
            _shieldCount = value;
            if(TitleManager.Instance != null)
                TitleManager.Instance.shieldTxt.text = (DataManager.userItem.shield - _shieldCount).ToString();
        }
    }
    public ShieldSelcetButtton[] selectButton;
    public void GameStart()
    {
        DataManager.Instance.useShieldCount = shieldCount;
        SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
    }
    public void Open()
    {
        shieldCount = 0;
        gameObject.SetActive(true);
        foreach (var i in selectButton)
            i.Init();
    }
    public void Close()
    {
        if(TitleManager.Instance != null)
            TitleManager.Instance.shieldTxt.text = DataManager.userItem.shield.ToString();
        gameObject.SetActive(false);
    }
}
