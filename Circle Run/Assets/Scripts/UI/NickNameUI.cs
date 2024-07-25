using UnityEngine;
using TMPro;
using System.Collections.Generic;
using BackEnd;
using System.Text;

public class NickNameUI : MonoBehaviour
{
    public TextMeshProUGUI errorTxt;
    public TMP_InputField nickNameInput;

    private void Awake()
    {
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
        nickNameInput.text = string.Empty;
    }
    public void NickNameSend()
    {
        LoadingManager.Instance.LoadingStart();
        string input = nickNameInput.text;
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        if (bytes.Length <= 3 || bytes.Length < 10)
        {
            //닉네임 길이 조절
            string key = "";
            LocalizationManager.Instance.ChangedTxt(key, errorTxt);
            errorTxt.gameObject.SetActive(true);
        }
        else
        {
            BackEndManager.Instance.SetNickName(nickNameInput.text, (res, error) =>
            {
                if (res)
                {
                    TitleManager.Instance.nickNameTxt.text = nickNameInput.text;
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
