using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }
    public static bool GetInitialization => CodelessIAPStoreListener.initializationComplete;
    public string Getprice(string id)
    {
        Debug.Log(CodelessIAPStoreListener.initializationComplete);
        Product product = CodelessIAPStoreListener.Instance.GetProduct(id);
        return product.metadata.localizedPriceString;
    }
    public void Restore(string id)
    {
        Product product = CodelessIAPStoreListener.Instance.GetProduct(id);
        if (product.hasReceipt)
        {
        }
        else
        {
        }
    }
}
