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
            if (TitleManager.Instance != null)
                TitleManager.Instance.shieldTxt.text = (DataManager.userItem.shield - _shieldCount).ToString();
        }
    }
    public ShieldSelcetButtton[] selectButton;
    public void GameStart()
    {
        LoadingManager.Instance.LoadingStart();
        BackEndManager.Instance.UseShield(shieldCount, (res) =>
        {
            LoadingManager.Instance.LoadingStop();
            if (res)
            {
                DataManager.Instance.useShieldCount = shieldCount;
                SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
            }
            else
            {
                InfoUI info = Instantiate(Resources.Load<InfoUI>("Prefabs/UI/InfoUI"));
                info.Open("Network_Error");
            }
        });
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
        if (TitleManager.Instance != null)
            TitleManager.Instance.shieldTxt.text = DataManager.userItem.shield.ToString();
        gameObject.SetActive(false);
    }
}
