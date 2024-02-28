using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterMain : MonoBehaviour  //인포를 상속,스탯을 가져옴
{
    MonsterInfo mon1 = new MonsterInfo("몬1",10.0f,15.0f,1,30.0f,0.5f, 30.0f);
   // MonsterInfo mon2 = new MonsterInfo();
    public enum State //유한상태머신 :: 스폰 -대기 -이동 -전투 -사망
    {
        Create,
        Idle,
        Move,
        Battle,
        Dead
    }
    public State m_State=State.Create;//몬스터 초기 상태를 스폰으로 설정

    Vector3 SpawnPos;  //몬스터의 스폰위치를 저장
    float MoveStartTime = 0.0f; //몬스터의 대기시간 
    void ChangeState(State s) //상태가 변할경우 변한상태를 전달받음
    {
        if (m_State == s) return; //상태가 변하지 않는경우 예외처리
         m_State = s; 
        switch (m_State)
        {
            case State.Idle: //몬스터가 대기상태
                m_Idle();
                break;
            case State.Move: //몬스터가 움직이는 상태
              
                m_Move();
                break;
            case State.Battle: //몬스터가 전투상태
                m_Battle();
                break;
            case State.Dead: //몬스터가 사망
                m_Dead();
                break;
        }
    }


    void m_Idle()
    {
        MoveStartTime = Random.Range(2, 6); //대기시간으로 전환된후 2-6초 경과후 움직이게 하기
        StartCoroutine(DelayChangeState(State.Move,MoveStartTime));
    }
    IEnumerator DelayChangeState(State s, float m_delaytime) //정해진값만큼 딜레이후 상태를 움직임으로 변경
    {
        yield return new WaitForSeconds(m_delaytime);
        ChangeState(s);
    }

    void m_Move()
    {

    }
    Vector3 GetRndPos()
    {
        Vector3 dir = Vector3.forward;
        dir = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0) * dir;
        dir *= Random.Range(0.0f, 3.0f);
        return SpawnPos + dir;
    }
    void m_Battle() 
    {

    }
    void m_Dead()
    {
        //플레이어에게 골드를 주거나 아이템을 드랍후 몬스터 삭제
        //gold += Random.Range(1, 10);
        //StopAllCoroutines();//모든 코룬틴 정지;
        Destroy(gameObject,10.0f*Time.deltaTime); //몬스터가 사망한경우 특정프레임이후 객체 삭제
    }


    // Start is called before the first frame update
    void Start()
    {
        SpawnPos= transform.position; //시작과 동시에 생성위치를 기억
        ChangeState(State.Idle); //몬스터를 대기상태로 변경
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
