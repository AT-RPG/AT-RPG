using AT_RPG;
using System;
using System.Collections;
using UnityEngine;
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

    public GameObject rageVFX;
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
        SetAttackOK(false);
        if (battleState != null) StopCoroutine(battleState);
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
    }
    public override void SkillUse()
    {
        SetSkillOk(false);
        if (battleState != null) StopCoroutine(battleState);
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
        myAnim.SetBool("Skill", true);
        StartCoroutine(Rage());
        StartCoroutine(skillCoolTimer());
        StartCoroutine(SkillmotionEnd());
    }

    IEnumerator SkillmotionEnd()
    {
        yield return new WaitForSeconds(5.0f);
        myAnim.SetBool("Skill", false);
        battleState = StartCoroutine(BattleState());
    }


    IEnumerator Rage()
    {
        GameObject RageVfx = Instantiate(rageVFX);  
        float buffTimer = 0.0f;
        baseBattleStat.attackPoint += 10;
        while (buffTimer < 30.0f)
        {   
            RageVfx.transform.position = this.gameObject.transform.position;  // 이펙트 포지션
            RageVfx.transform.rotation = Quaternion.identity;  // 이펙트 로테이션
            buffTimer += Time.deltaTime;
            yield return null;
        }
        Destroy(RageVfx);
        baseBattleStat.attackPoint -= 10;
    }
    IEnumerator skillCoolTimer()
    {
        float skillcoll = 0.0f;
        while (skillcoll <= baseBattleStat.skillCooltime)
        {
            skillcoll += Time.deltaTime;
            
            yield return null;
        }
        SetSkillOk(true);
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
            SetAttackOK(true);
            battleState = StartCoroutine(BattleState());
        }
    }

    void backwalk()
    {
        if (backstep != null) StopCoroutine(backstep);
        backstep = StartCoroutine(monBackWalk()); //뒤로이동
    }
    //통으로 새로짤것
    IEnumerator monBackWalk()
    {

        Vector3 battletarget = myTarget.transform.position;
        Vector3 dir = battletarget - transform.position;
        Vector3 moveDirection = -dir.normalized;
        float dist = dir.magnitude;
        
        while (dist < mStat.monsterRange)
        {
            Quaternion targetRotation = Quaternion.LookRotation(battletarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);

            Debug.Log("백스텝진입");
           myAnim.SetBool("BackWalk", true);
           monAgent.SetDestination(transform.position + moveDirection);
            

            // Update direction and distance
            battletarget = myTarget.transform.position;
            dir = battletarget - transform.position;
            dist = dir.magnitude;

           yield return new WaitForSeconds(1.0f);
          //yield return null;
        }
        
        Debug.Log("백스텝종료");
        myAnim.SetBool("BackWalk", false);
    }
    public override void OnAttack()
    {
        if (myTarget == null) return;
        shootManager.OnShoot(attackPos, baseBattleStat.attackPoint,myTarget.transform.position);
    }

    public void ballHit()
    {
        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        cd.TakeDamage(baseBattleStat.attackPoint);
    }

}
