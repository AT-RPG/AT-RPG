using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MeleeAttack : MonsterMain
{
    public override void AttackPlayer()
    {
        StopCoroutine(battleState());
        if (move != null)
        {
            StopCoroutine(move);
        }
        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
        AttackDeleay();
    }
    public override void AttackDeleay()
    {
        StartCoroutine(AttackDeleayState());
    }

    IEnumerator AttackDeleayState()
    {
        yield return new WaitForSeconds(baseBattleStat.attackDeley);
        StartCoroutine(battleState());
    }
}
