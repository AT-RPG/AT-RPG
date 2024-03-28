using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


public class Fireball : MonoBehaviour
{
    public IObjectPool<Fireball> firePool;
    [SerializeField]
    private float ballSpeed;

    private RangeAttack rangeattack;
    public void setMonsterMainInstance(RangeAttack instance) // 몬스터 메인의 인스턴스 설정
    {
        rangeattack = instance;
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
        transform.position = transform.parent.position;
        mShoot();
    }

    private void OnTriggerEnter(Collider other) //적에게 히트
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // 충돌한 오브젝트의 레이어가 몬스터 레이어인지 확인
        {
         rangeattack.ballHit(); //
        }
        destroyball();//릴리즈  
    }

    public void mShoot()
    {
        Invoke("destroyball", 6f);//충돌하지않을경우 6초후 릴리즈
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * ballSpeed); //발사
    }
    


}
