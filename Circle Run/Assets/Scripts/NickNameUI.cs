using UnityEngine;
using TMPro;

public class NickNameUI : MonoBehaviour
{
    public TextMeshProUGUI errorTxt;
    public TMP_InputField nickNameInput;

    private void Awake()
    {
        nickNameInput.onValueChanged.AddListener((input) => {
            if(errorTxt.gameObject.activeSelf)
                errorTxt.gameObject.SetActive(false);
            input.Replace(" ", "");
            input.Replace("\n", "");
            if (input.Length > 12)
                input.Remove(input.Length-1);
        });
    }
    private void OnEnable()
    {
        nickNameInput.text = string.Empty;
    }
    public void NickNameSend()
    {
        string input = nickNameInput.text;
        if (input.Length > 12 || input.Length < 4)
        {
            //���ö������̼����� ���� �޼��� ���� 
            string key = "";
            LocalizationManager.ChangedTxt(key, errorTxt);
            errorTxt.gameObject.SetActive(true);
        }
        else
        {
            BackEndManager.Instance.SetNickName(nickNameInput.text, (res, error) =>
            {
                if (res)
                    Close();
                else
                {
                    string key = " "; 
                    switch (error)
                    {
                        case "BadParameterException":
                            //20�� �̻� �� �г��ӿ� ������ ����
                            break;
                        case "DuplicatedParameterException":
                            //�ߺ�
                            break;

                        default:
                            break;
                    }
                    LocalizationManager.ChangedTxt(key, errorTxt);
                    errorTxt.gameObject.SetActive(true);
                    nickNameInput.text = string.Empty;
                }
            });
        }
    }
    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
