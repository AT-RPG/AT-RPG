using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangeAttack : MonsterMain
{
    [SerializeField]
    private GameObject FireballPrefab;

    private IObjectPool<Fireball> rangePool;

    private void Awake()
    {
        rangePool = new ObjectPool<Fireball>(spawnball, OnGetball, OnReleaseball, OnDestroyball, defaultCapacity: 5, maxSize: 5);
    }

    //프리팹을 복사하여 파이어볼 생성
    private Fireball spawnball()
    {
        Fireball fireball = Instantiate(FireballPrefab, transform).GetComponent<Fireball>(); 
        fireball.setManagedPool(rangePool);
        return fireball;
    }

    //풀에서 가져올때 호출
    private void OnGetball(Fireball fireball)
    {
        fireball.gameObject.SetActive(true);
    }
    //풀에 반환할때 호출
    private void OnReleaseball(Fireball fireball)
    {
        fireball.gameObject.SetActive(false);
    }
    //파괴(스택초과)될때 호출
    private void OnDestroyball(Fireball fireball)
    {
        Destroy(fireball.gameObject);
    }

    //발사
    public void OnShoot()
    {
       rangePool.Get();
    }

    public override void AttackPlayer()
    {
        if (battleState != null) StopCoroutine(battleState);
        if (move != null) StopCoroutine(move);

        myAnim.SetBool("Move", false);
        myAnim.SetBool("Run", false);
        myAnim.SetTrigger("NormalAttack");
        AttackDeleay();
    }
    public override void AttackDeleay()
    {
        //
        StartCoroutine(AttackDeleayState());
    }
    IEnumerator AttackDeleayState()
    {
        yield return new WaitForSeconds(baseBattleStat.attackDeley);
        if (monsterState != State.Dead) battleState = StartCoroutine(BattleState());
    }
    public override void OnAttack()
    {
        if (myTarget == null) return;
        Vector3 battletarget = myTarget.transform.position;
        Vector3 dir = battletarget - transform.position;
        float dist = dir.magnitude;
        
        OnShoot();
    }
    public void ballHit()
    {
        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        cd.TakeDamage(baseBattleStat.attackPoint);
    }
}
