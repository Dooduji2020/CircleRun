using System.Collections;
using UnityEngine;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    public TextMeshProUGUI loadingTxt;
    public bool isLoading = false;
    private const string loadingText = "Loading";
    private Canvas canvas;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        canvas = GetComponent<Canvas>();
    }
    public void LoadingStart()
    {
        canvas.sortingOrder = 10;
        isLoading = true;
        loadingTxt.text = loadingText;
        gameObject.SetActive(true);
        StartCoroutine(LoadingCoroutine());
    }
    IEnumerator LoadingCoroutine()
    {
        int length = loadingText.Length;
        while (isLoading)
        {
            yield return new WaitForSeconds(1f);
            int temp = loadingTxt.text.Length - length;
            if(temp >= 5)
                loadingTxt.text = loadingText;
            else
                loadingTxt.text += ".";
        }
        canvas.sortingOrder = -1;
        gameObject.SetActive(false);
        yield break;
    }

}
