using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangeAttack : MonoBehaviour
{
    ///몬스터 메인에서 공격->rangeattack스크립트실행(발사체 풀 관리 스크립트)->발사
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
    public void OnShoot(Vector3 fireballPosition, Vector3 fireballDirection)
    {
        var fireball = rangePool.Get();
   
    }
}
