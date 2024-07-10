using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager _Instance;
    public static LoadingManager Instance
    {
        get => _Instance;
        set
        {
            if(_Instance == null)
            { 
                if(FindObjectOfType<LoadingManager>())
                    _Instance = FindObjectOfType<LoadingManager>();
                else
                {
                    GameObject newObj = new GameObject(typeof(LoadingManager).Name);
                    _Instance = newObj.AddComponent<LoadingManager>();
                }
            }
        }
    }

    private LoadingUI loadingUI;

    public void LoadingStart()
    { 
        if(loadingUI == null)
            loadingUI = Instantiate(Resources.Load<LoadingUI>("Prefabs/UI/LoadingUI"));

        loadingUI.LoadingStart();
    }
    public void LoadingStop() => loadingUI.isLoading = false;
}
