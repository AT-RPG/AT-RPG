using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class MonsterSpwaner : MonoBehaviour
{
    [SerializeField]
    private GameObject _MonsterPrefab;

    private IObjectPool<MonsterMain> _Pool;

    private void Awake()
    {
        _Pool=new ObjectPool<MonsterMain>(spawnMonster,OnGetMonster,OnReleaseMonster,OnDestroyMonster,maxSize:5);
    }

    private MonsterMain spawnMonster()
    {
        MonsterMain monsterMain=Instantiate(_MonsterPrefab).GetComponent<MonsterMain>();
        monsterMain.setManagedPool(_Pool); 
        return monsterMain;
    }
    
    private void OnGetMonster(MonsterMain monsterMain)
    {
        monsterMain.gameObject.SetActive(true);
    }
    private void OnReleaseMonster(MonsterMain monsterMain)
    {
        monsterMain.gameObject.SetActive(false);
    }
    private void OnDestroyMonster(MonsterMain monsterMain)
    {
        Destroy(monsterMain.gameObject);
    }

    private void Start()
    {
        var monster = spawnMonster();
        _Pool.Release(monster);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var monsterImp=_Pool.Get();
        }
    }
}
