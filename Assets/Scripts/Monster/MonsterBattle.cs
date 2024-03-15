using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface MDamage
{
    void TakeDamage(float dmg);
}
[System.Serializable]
public struct MonsterStat
{
    public float monsterMoveSpeed;
    public float monsterRange;
    public float monsterDmage;
    public float monsterHp;
    public float monsterMaxHp;
    //공격속도
    public float monsterRpm;
    public float monsterIdleTime;
}

public class MonsterBattle : MonsterPorperty
{
    [SerializeField] public MonsterStat mStat;
    protected float curHP;
}

