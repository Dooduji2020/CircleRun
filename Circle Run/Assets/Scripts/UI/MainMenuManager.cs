using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }
    public StoreUI shopUI;
    public RankingUI rankingUI;
    public ShieldSelectUI shieldSelectUI;
    public OptionUI optionUI;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    private void Start()
    {
        AudioManager.Instance.AddButtonSound();
        //AuManager.Instance.AddButtonSound();
    }
    public void Init()
    {
        optionUI.Init();
        shopUI.Init();
    }
    public void ClickedPlay()
    {
        shieldSelectUI.Open();
    }
    public void OpenOption()
    {
        optionUI.Open();
    }
    public void ClickedQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ToggleSound()
    {
        // bool sound = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND) ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND)
        //      : 1) == 1;
        // sound = !sound;
        // PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, sound ? 1 : 0);
        // _soundImage.sprite = sound ? _activeSoundSprite : _inactiveSoundSprite;
        // //AudioManager.Instance.ToggleSound();
        // BgmManager.Instance.ToggleSound();
    }

    public void ShopOpen()
    {
        shopUI.gameObject.SetActive(true);
    }
    public void ShopClose()
    {
        shopUI.gameObject.SetActive(false);
    }
    public void RankingOpen()
    {
        rankingUI.gameObject.SetActive(true);
    }
    public void RankingClose()
    {
        rankingUI.gameObject.SetActive(false);
    }
}
