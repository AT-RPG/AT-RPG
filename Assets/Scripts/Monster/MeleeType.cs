using AT_RPG;
using System.Collections;
using UnityEngine;


public class MeleeType : MonsterMain
{
    public override void OnAttack()
    {
        if (myTarget == null) return;
        Vector3 battletarget = myTarget.transform.position;
        Vector3 dir = battletarget - transform.position;
        float dist = dir.magnitude;
        if (dist > mStat.monsterRange) return;
        Vector3 monsterForward = transform.forward;
        Vector3 playerDirection = dir.normalized;
        // 두 벡터 사이의 각도 계산
        float angle = Vector3.Angle(monsterForward, playerDirection);


        if (angle < 90.0f)//양옆으로 90도씩 정면180도가 범위
        {
            ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
            if (cd != null)
            {
                cd.TakeDamage(baseBattleStat.attackPoint);
            }
        }
    }

    public override void SkillUse()
    {
        SetSkillOk(false);
        if (battleState != null) StopCoroutine(battleState);
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
        myAnim.SetBool("Skill", true);
        StartCoroutine(guardSkill());
    }

    IEnumerator guardSkill()
    {
       float skillTimer = 0.0f;
        while (true)
        {
            Vector3 battletarget = myTarget.transform.position;
            Vector3 dir = battletarget - transform.position;
            Vector3 monsterForward = transform.forward;
            Vector3 playerDirection = dir.normalized;
            float angle = Vector3.Angle(monsterForward, playerDirection);
            if (angle < 90.0f)//양옆으로 90도씩 정면180도가 범위
            {
                //피해감소 스크립트 추가
            }
            skillTimer += Time.deltaTime;
            if (skillTimer >=4.0f) break;
            yield return null;
        }
        myAnim.SetBool("Skill", false);

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

    public override void AttackPlayer()
    {
        SetAttackOK(false);
        if (battleState!=null) StopCoroutine(battleState);   
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);

        myAnim.SetTrigger("NormalAttack");

    }
    public override void AttackDelay()
    {
        StartCoroutine(AttackDeleayState());
    }

    IEnumerator AttackDeleayState()
    {
        float timer=0.0f;
        while (true) 
        {
            Vector3 battletarget = myTarget.transform.position;
            Vector3 dir = battletarget - transform.position;
            float dist = dir.magnitude;
            // 목표 회전 각도를 계산합니다.
            Quaternion lookRotation = Quaternion.LookRotation(dir);

            // 천천히 회전하기 위해 Quaternion.Lerp()를 사용합니다.
            float rotationSpeed = 20f; // 회전 속도를 조절합니다.
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            timer += Time.deltaTime;
            if (timer >= baseBattleStat.attackDeley) break;
            yield return null;
        }
        SetAttackOK(true);
        if (monsterState !=State.Dead) battleState = StartCoroutine(BattleState());
    }
}
