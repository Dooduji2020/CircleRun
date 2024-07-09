using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingUI : MonoBehaviour
{
    private void Init()
    {
    }
    public void Open()
    {
        Init();
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
