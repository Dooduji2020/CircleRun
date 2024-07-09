using UnityEngine;
using UnityEngine.UI;


[System.Serializable]

public class BgmManager : MonoBehaviour {
  
  public static BgmManager Instance { get; private set; }


    
    [SerializeField]
    public AudioSource bgmPlayer;

    

    
    private bool isSoundMuted;
    private bool IsSoundMuted {

    get {

        isSoundMuted = (PlayerPrefs.HasKey(Constants.DATA.SETTINGS_SOUND)
            ? PlayerPrefs.GetInt(Constants.DATA.SETTINGS_SOUND) : 1) == 0;
        return isSoundMuted;
    }

    set {

        isSoundMuted = value;
        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, isSoundMuted ? 0 : 1);
    }
  }

    private void Awake() {

        if(Instance == null) {
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else {

            Destroy(gameObject);
            return;
        }

        PlayerPrefs.SetInt(Constants.DATA.SETTINGS_SOUND, IsSoundMuted ? 0 : 1);
        bgmPlayer.mute = IsSoundMuted;
    }

    public void AddButtonSound() {

        var buttons = FindObjectsOfType<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(() => {
                bgmPlayer.Play();
            });
        }
    }
    private void Start() {

        AddButtonSound();
    }

    public void PlayBGM() {

        //bgmPlayer.clip = bgmSound.clip;
        bgmPlayer.Play();
    }

    public void ToggleSound() {

        bgmPlayer.mute = IsSoundMuted;
    }
}
