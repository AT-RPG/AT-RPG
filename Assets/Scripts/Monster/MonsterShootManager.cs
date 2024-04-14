using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 몬스터가 쏘는 객체들을 생성해주고 관리합니다 
/// 
/// </summary>
public class MonsterShootManager : MonoBehaviour
{
    [SerializeField]
    private GameObject FireballPrefab;
    public Transform attackPos;
    private IObjectPool<Fireball> rangePool;

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
    public void OnShoot(Transform startPos,float damage)
    {
      
        Fireball fireball = rangePool.Get();
        fireball.SetDamage(damage);
        fireball.transform.position = startPos.position;
        fireball.transform.rotation = startPos.rotation;
        fireball.gameObject.SetActive(true);
    }
}
