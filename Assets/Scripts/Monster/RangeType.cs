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
    [SerializeField]
    private GameObject FireballPrefab;
    public Transform attackPos;
    private IObjectPool<Fireball> rangePool;

   public Coroutine backstep = null;
   public Coroutine attackdelay = null;

    private void Awake()
    {
        rangePool = new ObjectPool<Fireball>(spawnball, OnGetball, OnReleaseball, OnDestroyball, defaultCapacity: 5, maxSize: 5);
    }

    //프리팹을 복사하여 파이어볼 생성
    private Fireball spawnball()
    {
        Fireball fireball = Instantiate(FireballPrefab, transform).GetComponent<Fireball>();
        fireball.SetRangeAttackParent(this); // 부모 오브젝트 설정
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
        OnShoot();
    }

    public void ballHit()
    {
        ICharacterDamage cd = myTarget.GetComponent<ICharacterDamage>();
        cd.TakeDamage(baseBattleStat.attackPoint);
    }

}
