using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public static TitleManager Instance;
    public GameObject titleLogo;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        VersionCheckResult(BackEndManager.Instance.Init());
    }
    private void VersionCheckResult(bool result)
    {
        if (result)
        {
            StartCoroutine(LogoDelay());
        }
        else
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            Application.OpenURL("");
#elif !UNITY_EDITOR && UNITY_IOS
            Application.OpenURL("");
#endif
        }
    }
    public void UserDataInit()
    {
    }
    IEnumerator LogoDelay()
    {
        yield return new WaitForSeconds(1.5f);
        titleLogo.SetActive(false);
        yield break;
    }
}
