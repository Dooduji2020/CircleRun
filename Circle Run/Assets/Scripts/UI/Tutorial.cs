using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour
{
    private const string txtKey = "Tutorial_00";
    public TextMeshProUGUI tutorialTxt;
    public Button startBtn;
    private int index = 0;
    private bool delay = false;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        LocalizationManager.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
    }
    public void StepUp()
    {
        if (delay || index >= 5)
            return;
        delay = true;
        switch (index)
        {
            case 0:
                GameManager.Instance.player.canMove = true;
                LocalizationManager.ChangedTxt(txtKey + (index + 1).ToString(), tutorialTxt);
                break;
            case 1:
                GameManager.Instance.player.canShoot = true;
                LocalizationManager.ChangedTxt(txtKey + (index + 1).ToString(), tutorialTxt);
                break;
            case 2:
                break;
            case 3:
                LocalizationManager.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
                break;
            case 4:
                PlayerPrefs.SetInt("Tutorial", 1);
                LocalizationManager.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
                startBtn.gameObject.SetActive(true);
                break;
        }
        ++index;
        StartCoroutine(TutorialDelay());
        Debug.Log("StepUp");
    }
    IEnumerator TutorialDelay()
    {
        yield return new WaitForSeconds(4f);
        delay = false;
        //다음 진행 클릭하라고 진행
    }
    public void TutorialEndGameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
