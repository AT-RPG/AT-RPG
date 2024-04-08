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

        yield return new WaitForSeconds(baseBattleStat.attackDeley);
        if(monsterState !=State.Dead) battleState = StartCoroutine(BattleState());
    }
}
