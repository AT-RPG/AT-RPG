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

        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        if (cd != null)
        {
            cd.TakeDamage(baseBattleStat.attackPoint);
        }
    }
    public override void AttackPlayer()
    {
        if(battleState!=null) StopCoroutine(battleState);   
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
        if(monsterState !=State.Dead) battleState = StartCoroutine(BattleState());
    }
}
