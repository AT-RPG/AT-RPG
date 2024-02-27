using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Info : MonoBehaviour
{
  
    struct m_Info  //몬스터의 정보 구조체로 저장
    {
        string m_Name; //이름
        float m_Health; //체력
        float m_MoveSpeed; //이동속도
    }
    /*  ---------구조체랑 클래스중에 선택할것----------
    class M_Info //몬스터의 정보 클래스로 저장 
    {
        string m_Name;
        float m_Health; 
        float m_MoveSpeed;
    }
    */

    struct m_BattleInfo //몬스터의 공격
    {
        float m_Atk; //공격력
        float m_Rpm; //공격속도
        float m_Range; //사거리
    }

    /*-몬스터의 기본적인 내용구현후 보강할것
    struct m_Skill //몬스터의 스킬 
    {
        float m_Cooldown; //쿨타임
                          //   float m_UseMana; //사용 마나-몬스터에게 마나스탯을 부여할지 결정
    }
    */

}
