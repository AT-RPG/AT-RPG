using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct BattleStat
{
    public float AP;
    public float maxHP;
    public float AttackRange;
    public float AttackDelay;
}

public class BattleSystem : CharacterProperty
{
    [SerializeField] protected BattleStat battleStat;
    protected float curHP = 0.0f;
    protected float battleTime = 0.0f;
    public event UnityAction deathAlarm;
    Transform _target = null;
    protected Transform myTarget
    {
        get => _target;
        set 
        {
            _target = value;
            if(_target != null)
            {
                BattleSystem bs = _target.GetComponent<BattleSystem>();
                if(bs != null)
                {
                    bs.deathAlarm += TargetDead;
                }
            }
        }
    }

    void TargetDead()
    {
        StopAllCoroutines();
    }

    protected void Initialize()
    {
        curHP = battleStat.maxHP;
    }
    public void TakeDamage(float dmg)
    {
        curHP -= dmg;
        if (curHP <= 0.0f)
        {
            //Die
            OnDead();
            myAnim.SetTrigger("Dead");
        }
        else
        {
            myAnim.SetTrigger("Damage");
        }
    }

    public void OnAttack()
    {
        if (myTarget == null) return;
        BattleSystem bs = myTarget.GetComponent<BattleSystem>();
        if (bs != null)
        {
            bs.TakeDamage(battleStat.AP);
        }
    }

    protected virtual void OnDead()
    {
        deathAlarm?.Invoke();
        GetComponent<Collider>().enabled = false;
    }

    public bool IsLive()
    {
        return curHP > 0.0f;
    }
}
