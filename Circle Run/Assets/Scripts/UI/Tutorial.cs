using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Tutorial : MonoBehaviour
{
    private const string txtKey = "Tutorial_00";
    public TextMeshProUGUI tutorialTxt;
    public GameObject tutorialAnim;
    public Button startBtn;
    private int index = 0;
    private bool delay = false;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        tutorialAnim.SetActive(false);
        //GameManager.Instance.GamePlay();
        //LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
    }
    private void Start()
    {
        StartCoroutine(TutorialDelay());
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
                LocalizationManager.Instance.ChangedTxt(txtKey + (index + 1).ToString(), tutorialTxt);
                break;
            case 1:
                GameManager.Instance.player.canShoot = true;
                LocalizationManager.Instance.ChangedTxt(txtKey + (index + 1).ToString(), tutorialTxt);
                break;
            case 2:
                break;
            case 3:
                LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
                break;
            case 4:
                PlayerPrefs.SetInt("Tutorial", 1);
                LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
                startBtn.gameObject.SetActive(true);
                break;
        }
        ++index;
        StartCoroutine(TutorialDelay());
        Debug.Log("StepUp");
    }
    IEnumerator TutorialDelay()
    {
        int index = 0;
        LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
        ++index;
        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.player.canMove = true;
        LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
        yield return new WaitForSeconds(2f);

        while (index < 3)
        {
            if (index == 2)
            {
                GameManager.Instance.player.canShoot = true;
                tutorialAnim.SetActive(true);
            }
            LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
            ++index;
            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(2f);
        LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
        tutorialAnim.SetActive(false);
        GameManager.Instance.player.canShoot = false;
        GameManager.Instance.player.canMove = false;
        GameManager.Instance.TutorialStar();
        ++index;
        yield return new WaitForSeconds(2f);
        PlayerPrefs.SetInt("Tutorial", 1);
        LocalizationManager.Instance.ChangedTxt(txtKey + index.ToString(), tutorialTxt);
        startBtn.gameObject.SetActive(true);
        // if(index <= 4)
        //     tutorialAnim.SetActive(true);
        // delay = false;
        // StepUp();
        //다음 진행 클릭하라고 진행
    }
    public void TutorialEndGameStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
