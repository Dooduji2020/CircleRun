using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager _Instance;
    public static LoadingManager Instance
    {
        get
        {
            if(_Instance == null)
            { 
                if(FindObjectOfType<LoadingManager>())
                    _Instance = FindObjectOfType<LoadingManager>();
                else
                {
                    GameObject newObj = new GameObject(typeof(LoadingManager).Name);
                    _Instance = newObj.AddComponent<LoadingManager>();
                    DontDestroyOnLoad(_Instance.gameObject);
                }
            }
            return _Instance;
        }
    }

    private LoadingUI loadingUI;
    public void LoadingStart(bool isNetwork = false)
    { 
        if(loadingUI == null)
            loadingUI = Instantiate(Resources.Load<LoadingUI>("Prefabs/UI/LoadingUI"));

        loadingUI.LoadingStart();
    }
    public void LoadingStop() => loadingUI.isLoading = false;
}
