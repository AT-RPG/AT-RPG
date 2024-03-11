using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo
{
    public string monsterName;
    public float monsterHealth;
    public float monsterMoveSpeed;
    public int monsterLevel;

    public float mAtk;
    public float mRpm;
    public float mRange;

    public MonsterInfo(string monster_Name, float monster_Health, float monster_MoveSpeed, int monster_Level, float m_Atk, float m_Rpm, float m_Range)
    {
        monsterName = monster_Name;
        monsterHealth = monster_Health;
        monsterMoveSpeed = monster_MoveSpeed;
        monsterLevel = monster_Level;
        mAtk = m_Atk;
        mRpm = m_Rpm;
        mRange = m_Range;
    }

}
