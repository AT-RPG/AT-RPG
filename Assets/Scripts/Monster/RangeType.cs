using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;
using UnityEngine.EventSystems;

/// <summary>
/// <see cref="RangeType"/>
/// 원기리 몬스터 공격관리
/// </summary>
public class RangeType : MonsterMain
{

   public Coroutine backstep = null;
   public Coroutine attackdelay = null;
    public MonsterShootManager shootManager;

    public Transform attackPos;

    public override void OnEnable()
    { 
        base.OnEnable();
        GameObject managerObject = GameObject.Find("MonsterShootManager");
        if (managerObject != null)
        {
            shootManager = managerObject.GetComponent<MonsterShootManager>();
        }
    }
    public override void AttackPlayer()
    {
        if (battleState != null) StopCoroutine(battleState);
        monAgent.ResetPath();
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
    }
    public override void AttackDelay()
    {
        backwalk();  
        attackdelay = StartCoroutine(AttackDelayCoroutine());//공격딜레이 계산 코룬틴
    }

    IEnumerator AttackDelayCoroutine()
    {
        float attackDelay = baseBattleStat.attackDeley;

        while (attackDelay > 0f)
        {
            yield return null;
            attackDelay -= Time.deltaTime;
        }

        if (monsterState != State.Dead)
        {
            if (backstep != null) StopCoroutine(backstep);//뒤로이동중이라면 정지
            myAnim.SetBool("BackWalk", false);
            monAgent.ResetPath();
            battleState = StartCoroutine(BattleState());
        }
    }

    void backwalk()
    {
        if (backstep != null) StopCoroutine(backstep);
        backstep = StartCoroutine(monBackWalk()); //뒤로이동
    }
    IEnumerator monBackWalk()
    {

        Vector3 battletarget = myTarget.transform.position;
        Vector3 dir = battletarget - transform.position;
        float dist = dir.magnitude;

        while (dist < mStat.monsterRange && monsterState != State.Dead)
        {
            transform.LookAt(myTarget);
            myAnim.SetBool("BackWalk", true);
            Vector3 moveDirection = -dir.normalized;
            monAgent.SetDestination(transform.position + moveDirection);
            yield return null;
            dir = battletarget - transform.position;
            dist = dir.magnitude;
            myAnim.SetBool("BackWalk", false);
        }
        transform.LookAt(myTarget);
        backwalk();
    }
    public override void OnAttack()
    {
        if (myTarget == null) return;
        shootManager.OnShoot(attackPos);
    }

    public void ballHit()
    {
        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        cd.TakeDamage(baseBattleStat.attackPoint);
    }

}
