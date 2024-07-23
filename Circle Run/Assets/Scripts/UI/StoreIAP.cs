using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class StroreIAP : MonoBehaviour
{
    public TextMeshProUGUI priceTxt;
    public string productID;

    private CodelessIAPButton iapButton;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.interactable = false;
        iapButton = GetComponent<CodelessIAPButton>();
        iapButton.onPurchaseComplete.AddListener(PurchaseComplete);
        iapButton.onPurchaseFailed.AddListener(PurchaseFailed);
        priceTxt.text = "Loading..";
        StartCoroutine(PriceInit());
    }
    private void PurchaseComplete(Product product)
    {
        LoadingManager.Instance.LoadingStart();
        Debug.Log(product.transactionID);
        Debug.Log(product.definition.id);
        switch (product.definition.id)
        {
            case "001":
                DataManager.userItem.adsRemove = true;
                // 광고 제거
                break;
            case "002":
                DataManager.userItem.continueCoupon += 5;
                // 이어하기 5
                break;
            case "003":
                // 쉴드 5
                DataManager.userItem.shield += 5;
                break;
            case "004":
                // 패키지 쉴 20 이어하기 15
                DataManager.userItem.shield += 20;
                DataManager.userItem.continueCoupon += 15;
                break;
            case "005":
                // 쉴드 2 이어하기 2  인게임 상품
                DataManager.userItem.shield += 2;
                DataManager.userItem.continueCoupon += 2;
                break;
            case "s_003":
                // 쉴드 20
                DataManager.userItem.shield += 20;
                break;
            case "c_002":
                // 컨티뉴 25
                DataManager.userItem.continueCoupon += 25;
                break;
        }
        BackEndManager.Instance.ItemDataUpdate((result) =>
        {

            InfoUI info = Instantiate(Resources.Load<InfoUI>("Prefabs/UI/InfoUI"));
            if (result)
                info.Open("Inapp_Success");
            LoadingManager.Instance.LoadingStop();
        });
        if (TitleManager.Instance != null)
            TitleManager.Instance.ItemUISet();

    }
    private void PurchaseFailed(Product product, PurchaseFailureDescription description)
    {

    }
    private IEnumerator PriceInit()
    {
        while (!NetworkManager.GetInitialization)
            yield return null;
        if (productID.Equals("001") && DataManager.userItem.adsRemove)
        {
            LocalizationManager.Instance.ChangedTxt("Inapp_Success", priceTxt);
        }
        else
        {
            priceTxt.text = NetworkManager.Instance.Getprice(productID);
            button.interactable = true;
        }
    }
}
