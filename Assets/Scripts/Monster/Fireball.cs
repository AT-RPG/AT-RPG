using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Fireball : MonoBehaviour
{
    public IObjectPool<Fireball> firePool;
    [SerializeField]
    private float ballSpeed;
    private Vector3 direction;

    private MonsterMain monsterMainInstance;
    public void setMonsterMainInstance(MonsterMain instance) // 몬스터 메인의 인스턴스 설정
    {
        monsterMainInstance = instance;
    }


    public void setManagedPool(IObjectPool<Fireball> pool) //풀설정
    {
        firePool = pool;
    }
    public void destroyball() //풀반환
    {
        firePool.Release(this);
    }

    /*
    private void Awake() //파괴된게 재호출될땐 여기
    {
        
    }
    void OnEnable()//릴리스된게 재호출될땐 여기
    {
        
    }
    */

    private void OnTriggerEnter(Collider other) //적에게 히트
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player")) // 충돌한 오브젝트의 레이어가 몬스터 레이어인지 확인
        {
            if (monsterMainInstance != null) // 몬스터 메인의 인스턴스가 유효한지 확인
            {
           //     monsterMainInstance.TakeDamage(); // 몬스터 메인의 TakeDamage() 함수 호출
            }
        }
        destroyball();//릴리즈
    }

    public void mShoot(Vector3 dir)
    {
        direction = dir;
        Invoke("destroyball", 6f);//충돌하지않을경우 6초후 릴리즈
    }

    private void Update()
    {
        //  transform.Translate(direction*Time.deltaTime*ballSpeed); //발사
        transform.Translate(Vector3.forward * Time.deltaTime * ballSpeed); //발사
    }
    


}
