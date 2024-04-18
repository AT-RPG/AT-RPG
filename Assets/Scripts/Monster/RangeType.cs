using AT_RPG;
using System.Collections;
using UnityEngine;

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
    public void lookPlayer()
    {
        Vector3 directionToPlayer = (myTarget.transform.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3.0f);
    }
    public override void AttackPlayer()
    {
        SetAttackOK(false);
        if (battleState != null) StopCoroutine(battleState);
        monAgent.ResetPath();

        lookPlayer();
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
        myAnim.SetBool("Skill", true);
        myAnim.SetTrigger("NormalAttack");
        
        Debug.Log("스킬사용");
        StartCoroutine(Rage());
    }
    IEnumerator Rage()
    {
        myAnim.SetBool("Skill", false);
        float buffTimer = 0.0f;
        while (true)
        {
            baseBattleStat.attackPoint += 10;
            buffTimer += Time.deltaTime;
            if (buffTimer > 30.0f) break;
            yield return null;
        }
        baseBattleStat.attackPoint -= 10;
        if (monsterState != State.Dead)
        {
            StartCoroutine(skillCoolTimer());
            battleState = StartCoroutine(BattleState());
        }
    }
    IEnumerator skillCoolTimer()
    {
        float skillcoll = 0.0f;
        while (true)
        {
            skillcoll += Time.deltaTime;
            if (skillcoll >= baseBattleStat.skillCooltime) break;
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
    IEnumerator monBackWalk()
    {
        while (true)
        {
            if (myTarget == null || monsterState == State.Dead)
                yield break;

            Vector3 battletarget = myTarget.transform.position;
            Vector3 dir = battletarget - transform.position;
            float dist = dir.magnitude;
            if (dist < mStat.monsterRange && monsterState != State.Dead)
            {
                myAnim.SetBool("BackWalk", true);
                Vector3 moveDirection = -dir.normalized;
                lookPlayer();
                monAgent.SetDestination(transform.position + moveDirection);
            }
            else
            {
                monAgent.ResetPath();
                myAnim.SetBool("BackWalk", false);
                break;
            }
            yield return null;
        }
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
