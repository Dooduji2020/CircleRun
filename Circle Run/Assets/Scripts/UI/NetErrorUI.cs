using System;
using UnityEngine;
using UnityEngine.UI;

public class NetErrorUI : MonoBehaviour
{
    public Button retryButton;
    private Canvas myCanvas;
    private event Action retryEvent;
    private int retryCount = 0;
    private void Awake()
    {
        myCanvas = GetComponent<Canvas>();
        retryButton.onClick.AddListener(Retry);
    }
    public void Init(Action _event)
    {
        retryEvent = null;
        if (retryCount >= 3)
        {
        }
        else
        {
            if (myCanvas == null)
                myCanvas = GetComponent<Canvas>();
            myCanvas.sortingOrder = 20;
            gameObject.SetActive(true);
            retryEvent += _event;
        }
    }
    private void Retry()
    {
        ++retryCount;
        myCanvas.sortingOrder = 0;
        gameObject.SetActive(false);

        retryEvent?.Invoke();
    }
}

