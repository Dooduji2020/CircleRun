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
        Invoke("Init", 2f);
    }
    private void Init()
    {
        if (!BackEndManager.isInit)
            VersionCheckResult(BackEndManager.Instance.Init());
        else
            titleLogo.SetActive(false);
    }
    private void VersionCheckResult(bool result)
    {
        if (result)
        {
            BackEndManager.Instance.GetRanking();
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
        float timer = 0;
        while (BackEnd.SendQueue.UnprocessedFuncCount > 0 && timer < 5f)
        {
            timer += Time.deltaTime;
            yield return null; 
        }
        if(timer >= 5f && BackEnd.SendQueue.UnprocessedFuncCount > 0)
        {
            Debug.Log("·©Å· ´Ù ¸øºÒ·µÀ½");
            //·©Å· Á¤º¸ ´Ù ¸øºÒ·¯¿È
            Debug.Log(DataManager.userScore.DailyScore);
        }
        else
        {
            Debug.Log("·©Å· ´Ù Àß °¡Á®¿È");
        }
        titleLogo.SetActive(false);
        yield break;
    }
}
