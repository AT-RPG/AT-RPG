using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Fireball : MonoBehaviour
{
    public IObjectPool<Fireball> firePool;

    public void setManagedPool(IObjectPool<Fireball> pool) //풀설정
    {
        firePool = pool;
    }
    public void destroyball() //풀반환
    {
        firePool.Release(this);
    }
    private void Awake() //파괴된게 재호출될땐 여기
    {
        
    }
    void OnEnable()//릴리스된게 재호출될땐 여기
    {
        
    }

    private void Update()
    {
        
    }


}
