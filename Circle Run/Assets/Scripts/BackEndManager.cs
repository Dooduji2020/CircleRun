using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;

public class BackEndManager : MonoBehaviour
{
    private void Start()
    {
        var bro = Backend.Initialize(true, true);
        if(bro.IsSuccess())
        { 
            //�ʱ�ȭ ����
        }
        else
        { 
            //�ʱ�ȭ ����
        }
    }
    public void GetRanking()
    {
        //�ش� ��ŷ ���̺��� 15�������� ������ �����´�
        Backend.URank.User.GetRankList("tableUUID", 15);
    }
}
