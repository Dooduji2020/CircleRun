using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class NickNameUI : MonoBehaviour
{
    public TextMeshProUGUI errorTxt;
    public TMP_InputField nickNameInput;
    public Button sendButton;

    private void Awake()
    {
        nickNameInput.onValueChanged.AddListener((input) => NickNameCheck(input));
        nickNameInput.onEndEdit.AddListener((input) =>
        {
            if (errorTxt.gameObject.activeSelf)
                errorTxt.gameObject.SetActive(false);
            input.Replace(" ", "");
            input.Replace("\n", "");
        });
    }
    private void OnEnable()
    {
        nickNameInput.textComponent.color = Color.black;
        nickNameInput.text = string.Empty;
    }
    private void NickNameCheck(string nick)
    {
        int bytes = Encoding.UTF8.GetByteCount(nick);
        Debug.Log(bytes);
        if (nick.Length < 2 && bytes <= 3)
        {
            nickNameInput.textComponent.color = Color.red;
            sendButton.gameObject.SetActive(false);
        }
        else if (nick.Length >= 2 && bytes >= 4)
        {
            nickNameInput.textComponent.color = Color.green;
            sendButton.gameObject.SetActive(true);
        }
    }
    public void NickNameSend()
    {
        //LoadingManager.Instance.LoadingStart();
        string input = nickNameInput.text;
        int bytes = Encoding.UTF8.GetByteCount(input);
        Debug.Log(bytes);
        Debug.Log(input.Length);
        if (input.Length < 2 && bytes < 4)
        {
            LocalizationManager.Instance.ChangedTxt("NickName_Info", errorTxt);
            errorTxt.gameObject.SetActive(true);
        }
        else
        {
            BackEndManager.Instance.SetNickName(nickNameInput.text, (res, error) =>
            {
                if (res)
                {
                    TitleManager.Instance.nickNameTxt.text = nickNameInput.text;
                    OptionUI.Instance.nickNameTxt.text = nickNameInput.text;
                    Close();
                }
                else
                {
                    string key = "NickName_Fail";
                    switch (error)
                    {
                        case "BadParameterException":  // 공백 등
                        key = "NickName_Fail";
                            break;
                        case "DuplicatedParameterException":   // 중복
                        key = "NickName_Duplication";
                            break;

                    }
                    LocalizationManager.Instance.ChangedTxt(key, errorTxt);
                    errorTxt.gameObject.SetActive(true);
                    nickNameInput.text = string.Empty;
                }
            });
        }
        LoadingManager.Instance.LoadingStop();
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
