using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class NetworkManager : MonoBehaviour, IDetailedStoreListener
{
    public static NetworkManager Instance { get; private set; }
    private IStoreController _controller;
    private IExtensionProvider _extensions;
    private string[] productID = { 
        "dongrami_coupon2", "001", "dongrami_shield2", "dongrami_coupon1" ,
        "dongrami_shield5","dongrami_itemset","dongrami_ingameset"
    };
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
    public void IAPInit()
    {
        if (GetInitialization)
            return;
            

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach(var i in productID)
        {
            if(!i.Equals("001"))
                builder.AddProduct(i, ProductType.Consumable);
            else
                builder.AddProduct(i, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }
    public string Getprice(string id)
    {
        try
        {
            Debug.Log(CodelessIAPStoreListener.initializationComplete);
            Product product = CodelessIAPStoreListener.Instance.GetProduct(id);
            return product.metadata.localizedPriceString;
        }
        catch
        {
            return "";
        }
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

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        throw new System.NotImplementedException();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        throw new System.NotImplementedException();
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        CodelessIAPStoreListener.Instance.OnInitialized(controller, extensions);
        _controller = controller;
        _extensions = extensions;
    }
}
