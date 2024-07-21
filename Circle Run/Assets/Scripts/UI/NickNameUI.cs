using UnityEngine;
using TMPro;
using System.Collections.Generic;
using BackEnd;

public class NickNameUI : MonoBehaviour
{
    public TextMeshProUGUI errorTxt;
    public TMP_InputField nickNameInput;

    private void Awake()
    {
        nickNameInput.onValueChanged.AddListener((input) =>
        {
            if (errorTxt.gameObject.activeSelf)
                errorTxt.gameObject.SetActive(false);
            input.Replace(" ", "");
            input.Replace("\n", "");
            if (input.Length > 12)
                input.Remove(input.Length - 1);
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
        if (input.Length > 12 || input.Length < 4)
        {
            //닉네임 길이 조절
            string key = "";
            LocalizationManager.ChangedTxt(key, errorTxt);
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
                    LocalizationManager.ChangedTxt(key, errorTxt);
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
