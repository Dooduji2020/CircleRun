using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InfoUI : MonoBehaviour
{
    private Canvas canvas;

    public Button closeButton;
    public TextMeshProUGUI infoTxt;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        closeButton.interactable = false;
        closeButton.onClick.AddListener(Close);
    }
    public void Open(string key)
    {
        if (canvas == null)
            canvas = GetComponent<Canvas>();
        StartCoroutine(CloseDelay());
        gameObject.SetActive(true);
        canvas.sortingOrder = 2;
        LocalizationManager.ChangedTxt(key, infoTxt);
    }
    private void Close()
    {
        canvas.sortingOrder = -1;
        this.gameObject.SetActive(false);
        closeButton.interactable = false;
    }
    IEnumerator CloseDelay()
    {
        yield return new WaitForSeconds(0.5f);
        closeButton.interactable = true;
        yield break;
    }
}
