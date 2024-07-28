using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _effectSource;
    //private AudioSource _bgmSound;

    [SerializeField]
    private AudioClip _clickSound;
    [SerializeField]
    private AudioClip _shieldSound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _effectSource.mute = PlayerPrefs.GetInt("Sound", 1) > 0 ? false : true;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    public void AddButtonSound()
    {
        var buttons = FindObjectsOfType<Button>(true);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.AddListener(() =>
            {
                PlaySound(_clickSound);
            });
        }
    }
    public void ShieldSound()
    {
        _effectSource.PlayOneShot(_shieldSound);
    }

    public void PlaySound(AudioClip clip)
    {
        _effectSource.PlayOneShot(clip);
    }
    public void SoundMute(bool mute)
    {
        _effectSource.mute = mute;
    }
}