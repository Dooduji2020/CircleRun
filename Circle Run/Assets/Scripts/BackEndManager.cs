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
            //초기화 성공
        }
        else
        { 
            //초기화 실패
        }
    }
    public void GetRanking()
    {
        //해당 랭킹 테이블의 15위까지의 순위를 가져온다
        Backend.URank.User.GetRankList("tableUUID", 15);
    }
}
