using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonsterAI : MonoBehaviour
{
    public UnityEvent<Transform> findPlayer;
    public UnityEvent lostPlayer;
    public LayerMask mask;
    public Transform myTarget;




    private void OnTriggerEnter(Collider other) //몬스터의 트리거 작동
    {
     
        if ((mask & 1 << other.gameObject.layer) != 0) //충돌한 레이어가 몬스터가 반응할 레이어인지 판단
        {
            
            if (myTarget == null) //타겟이 없을경우
            {
               
                myTarget = other.transform; //타겟을 충돌대상의 위치로 바꾼다
                findPlayer?.Invoke(myTarget); //후에 findPlayer 함수 실행
            }
        }
    }

    private void OnTriggerExit(Collider other) //충돌이 종료됨
    {
        if (myTarget == other.transform) //타겟이 플레이어 였던경우
        {
            myTarget = null; //타겟을 초기화
            lostPlayer?.Invoke(); //lostPlayer함수 실행
        }
    }
}
