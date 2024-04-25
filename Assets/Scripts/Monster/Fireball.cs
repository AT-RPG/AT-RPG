using AT_RPG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 파이어볼-투사체 관리스크립트
/// </summary>
public class Fireball : MonoBehaviour, ICharacterDamage
{
   
    public IObjectPool<Fireball> firePool;
    Coroutine stop = null;
    [SerializeField]
    private float ballSpeed;
    private float damage;

    private Vector3 direction;

    private MonsterShootManager monsterShootManager;
    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }
    public void setStartPos(Transform myPos)
    {
        transform.position = myPos.transform.position;
    }
    public void SetTarget(Vector3 target)
    {
        direction = (target - transform.position).normalized;
    }


    public void SetRangeAttackParent(MonsterShootManager parent)
    {
        monsterShootManager = parent;
    }
    public void setMonsterMainInstance(MonsterShootManager instance) // 몬스터 메인의 인스턴스 설정
    {
        monsterShootManager = instance;
    }

    public void setManagedPool(IObjectPool<Fireball> pool) //풀설정
    {
        firePool = pool;
    }
    public void destroyball() //풀반환
    {
        firePool.Release(this);
    }

    private void OnEnable()
    {
        stop = StartCoroutine(ShootLost());
    }

    private void OnTriggerEnter(Collider other) //적에게 히트
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Monster")) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // 충돌한 오브젝트의 레이어가 몬스터 레이어인지 확인
        {
            ICharacterDamage character = other.GetComponent<ICharacterDamage>();
            if (character != null)
            {
                character.TakeDamage(damage); // 맞은 대상에게 데미지를 줌
            }
        }
        StopCoroutine(stop);
        destroyball();//릴리즈  
    }

    IEnumerator ShootLost()
    {
        yield return new WaitForSeconds(3f);
        destroyball();
    }
    private void Start()
    {
        
    }
    private void Update()
    {
        transform.Translate(direction * Time.deltaTime * ballSpeed);

    }

    public void TakeDamage(float dmg)
    {
        Destroy(gameObject);
    }
}
