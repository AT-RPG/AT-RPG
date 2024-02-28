using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    string monsterName;
    float monsterHealth;
    float monsterMoveSpeed;
    int monsterLevel;

    float mAtk; //공격력
    float mRpm; //공격속도
    float mRange; //사거리
    /*  ---------구조체랑 클래스중에 선택할것----------
     struct m_Info  //몬스터의 정보 구조체로 저장
     {
         string m_Name; //이름
         float m_Health; //체력
         float m_MoveSpeed; //이동속도
         int m_Level; //레벨
     }

     struct m_BattleInfo //몬스터의 공격
     {
         float m_Atk; //공격력
         float m_Rpm; //공격속도
         float m_Range; //사거리
     }
    */
    public MonsterInfo(string monster_Name, float monster_Health, float monster_MoveSpeed, int monster_Level, float m_Atk, float m_Rpm ,float m_Range) 
    {
        monsterName = monster_Name;
        monsterHealth = monster_Health;
        monsterMoveSpeed = monster_MoveSpeed;
        monsterLevel = monster_Level;
        mAtk = m_Atk;
        mRpm = m_Rpm;
        mRange = m_Range;
    } //생성자

}
