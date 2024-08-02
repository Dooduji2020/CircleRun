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
        if (shieldCount > 0)
        {
            BackEndManager.Instance.UseShield(shieldCount, (res) =>
          {

              if (res)
              {
                  DataManager.Instance.useShieldCount = shieldCount;
                  if (DataManager.Instance.useShieldCount == 0)
                  {
                      DataManager.timeData.Shield = BackEndManager.Instance.GetTime();
                      BackEndManager.Instance.GetTimeUpdate(DataManager.Instance.GetTimeParam(), DataManager.timeData.inDate, () =>
                      {
                          LoadingManager.Instance.LoadingStop();
                          SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
                      });
                  }
                  else
                  {
                      LoadingManager.Instance.LoadingStop();
                      SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
                  }
              }
              else
              {
                  InfoUI info = Instantiate(Resources.Load<InfoUI>("Prefabs/UI/InfoUI"));
                  info.Open("Network_Error");
                  LoadingManager.Instance.LoadingStop();
              }
          });
        }
        else
            LoadingManager.Instance.LoadingStop();
            SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
    }
    public void Open()
    {
        if (PlayerPrefs.GetInt("Tutorial", 0) == 0) SceneManager.LoadScene(Constants.DATA.GAMEPLAY_SCENE);
        else
        {
            shieldCount = 0;
            gameObject.SetActive(true);
            foreach (var i in selectButton)
                i.Init();
        }
    }
    public void Close()
    {
        if (TitleManager.Instance != null)
            TitleManager.Instance.shieldTxt.text = DataManager.userItem.shield.ToString();
        gameObject.SetActive(false);
    }
}
