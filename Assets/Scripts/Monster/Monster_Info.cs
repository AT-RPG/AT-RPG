using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    string monsterName;
    float monsterHealth;
    float monsterMoveSpeed;
    int monsterLevel;

    float mAtk; //���ݷ�
    float mRpm; //���ݼӵ�
    float mRange; //��Ÿ�
    /*  ---------����ü�� Ŭ�����߿� �����Ұ�----------
     struct m_Info  //������ ���� ����ü�� ����
     {
         string m_Name; //�̸�
         float m_Health; //ü��
         float m_MoveSpeed; //�̵��ӵ�
         int m_Level; //����
     }

     struct m_BattleInfo //������ ����
     {
         float m_Atk; //���ݷ�
         float m_Rpm; //���ݼӵ�
         float m_Range; //��Ÿ�
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
    } //������

}
