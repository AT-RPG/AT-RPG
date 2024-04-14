using AT_RPG;
using System;
using UnityEngine;
using UnityEngine.Events;

public class CommonBattle : CharacterProperty, ICharacterDamage
{
    [SerializeField]
    protected BaseBattleStat baseBattleStat;
    public UnityEvent<float> changeHpAct;
    public event UnityAction deathAlarm;
    float _curHP = 0.0f;
    protected float curHP
    {
        get => _curHP;
        set
        {
            _curHP = value;
            changeHpAct?.Invoke(_curHP / baseBattleStat.maxHP);
        }
    }

    Transform _target = null;
    protected Transform myTarget
    {
        get => _target;
        set 
        {
            _target = value;
            if(_target != null)
            {
                CommonBattle cb = _target.GetComponent<CommonBattle>();
                if(cb != null)
                {
                    cb.deathAlarm += TargetDead;
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
        curHP = baseBattleStat.maxHP / 2.0f;
        Debug.Log($"Common / baseBattleStat.maxHP {baseBattleStat.maxHP}");
        Debug.Log($"Common / baseBattleStat.attackPoint {baseBattleStat.attackPoint}");
        Debug.Log($"Common / baseBattleStat.attackDeley {baseBattleStat.attackDeley}");
        Debug.Log($"Common / baseBattleStat.moveSpeed {baseBattleStat.moveSpeed}");
        Debug.Log($"Common / baseBattleStat.skillCooltime {baseBattleStat.skillCooltime}");
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
            //Damage
            myAnim.SetTrigger("Damage");
            SetDamageEffect();
        }
    }

    public virtual void SetDamageEffect()
    {

    }
    public virtual void OnAttack()
    { 

    }

    protected virtual void OnDead()
    {
       deathAlarm?.Invoke();
    }
}
