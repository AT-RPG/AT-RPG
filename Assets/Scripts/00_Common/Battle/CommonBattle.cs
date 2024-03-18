using AT_RPG;
using UnityEngine;
using UnityEngine.Events;

public class CommonBattle : CharacterProperty, ICharacterDamage
{
    BaseBattleStat baseBattleStat;
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
        curHP = baseBattleStat.maxHP;
    }

    public void TakeDamage(float dmg)
    {
        curHP -= dmg;
        if (curHP <= 0.0f)
        {
            //Die
        }
        else
        {
            //Damage
        }
    }

    public void OnAttack()
    {
        if (myTarget == null) return;
        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        if (cd != null)
        {
            cd.TakeDamage(baseBattleStat.attackPoint);
        }
    }

    protected virtual void OnDead()
    {
        deathAlarm?.Invoke();
    }
}
