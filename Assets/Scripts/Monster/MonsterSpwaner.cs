using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MonsterSpwaner : MonoBehaviour
{

    [SerializeField] private GameObject _impPrefab; //생성할 프리팹을 저장
    [SerializeField] private GameObject _knightPrefab;
    [SerializeField] private GameObject _monkeyPrefab;
    [SerializeField] private GameObject _pitboyPrefab;
    [SerializeField] private GameObject _nosdmanPrefab;
    [SerializeField] private GameObject _spikePrefab;
    [SerializeField] private GameObject _treestorPrefab;

    private GameObject _MonsterPrefab;




    [SerializeField] public int monnum = 0;

    
    private IObjectPool<MonsterMain> _Pool;

    //풀을 생성하고 초기화
    private void Awake()
    {
        _Pool = new ObjectPool<MonsterMain>(spawnMonster, OnGetMonster, OnReleaseMonster, OnDestroyMonster, defaultCapacity:5, maxSize: 5) ;
    }

    //프리팹을 복사하여 몬스터를 생성하고 반환
    private MonsterMain spawnMonster()
    {
        if (monnum == 0) _MonsterPrefab = _impPrefab;
        else if (monnum == 1) _MonsterPrefab = _knightPrefab;
        else if (monnum == 2) _MonsterPrefab = _monkeyPrefab;
        else if (monnum == 3) _MonsterPrefab = _pitboyPrefab;
        else if (monnum == 4) _MonsterPrefab = _nosdmanPrefab;
        else if (monnum == 5) _MonsterPrefab = _spikePrefab;
        else if (monnum == 6) _MonsterPrefab = _treestorPrefab;
        else _MonsterPrefab = _impPrefab;

        MonsterMain monsterMain=Instantiate(_MonsterPrefab, transform).GetComponent<MonsterMain>(); //인자로 스포너의 위치를전달
        monsterMain.setManagedPool(_Pool); 
        return monsterMain;
    }
    
    //몬스터를 풀에서 가져올때 호출
    private void OnGetMonster(MonsterMain monsterMain)
    {
        monsterMain.gameObject.SetActive(true);
    }
    //몬스터를 풀에 반환할때 호출
    private void OnReleaseMonster(MonsterMain monsterMain)
    {
        monsterMain.gameObject.SetActive(false);
    }
    //몬스터가 파괴(스택초과)될때 호출
    private void OnDestroyMonster(MonsterMain monsterMain)
    {
        Destroy(monsterMain.gameObject);
    }

   
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) //생성
        {
           _Pool.Get();
        }
    }
}
