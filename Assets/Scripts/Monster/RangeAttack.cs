using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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
        transform.LookAt(myTarget); //플레이어를 보도록 회전
        myAnim.SetTrigger("NormalAttack");
    }
    public override void AttackDelay()
    {
        StartCoroutine(monBackWalk()); //뒤로이동
        StartCoroutine(AttackDelayCoroutine());//공격딜레이 계산 코룬틴
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
            if(monBackWalk()!=null)StopCoroutine(monBackWalk());//뒤로이동중이라면 정지
            myAnim.SetBool("BackWalk", false);
            battleState = StartCoroutine(BattleState());
        }
    }

    IEnumerator monBackWalk()
    {
        Vector3 battletarget = myTarget.transform.position;
        Vector3 dir = battletarget - transform.position;
        float dist = dir.magnitude;
        
        if (dist < mStat.monsterRange) //플레이어와 거리가 사거리보다 짧으면 뒷걸음시작
        {
            while (monsterState != State.Dead && dist < mStat.monsterRange)
            {
                transform.LookAt(myTarget);
                myAnim.SetBool("BackWalk", true);
                Vector3 moveDirection = -dir.normalized;
                monAgent.SetDestination(transform.position + moveDirection);
                // 플레이어와의 거리를 다시 계산합니다.
                dir = battletarget - transform.position;
                dist = dir.magnitude;
                yield return null;
            }
            myAnim.SetBool("BackWalk", false);
        }
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
