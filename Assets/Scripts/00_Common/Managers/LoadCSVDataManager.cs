using UnityEngine;
using AT_RPG;

public class LoadCSVDataManager
{
    // 드랍아이템 CSV
    private string[] dropItemDatas;
    public string[] DropItemDatas
    {
        get { return dropItemDatas; }
    }

    // 몬스터정보 CSV
    private string[] monsterDatas;
    public string[] MonsterDatas
    {
        get { return monsterDatas; }
    }

    // 스폰위치정보 CSV
    private string[] spawnPointDatas;
    public string[] SpawnPointDatas
    {
        get { return spawnPointDatas; }
    }
    
    public LoadCSVDataManager()
    {
        LoadAllResourcesData();
    }

    private void LoadAllResourcesData()
    {
        // 테이블 데이터 읽기
        string path             = Application.dataPath + "/Resources/CSVData/";
        string data_dripitem    = "JJappalWorld - DropItemData.csv";
        // string data_monster     = "";
        // string data_spawnpoint  = "";
        dropItemDatas           = System.IO.File.ReadAllLines(path + data_dripitem);
        // spawnPointDatas         = System.IO.File.ReadAllLines(path + data_spawnpoint);
        // monsterDatas            = System.IO.File.ReadAllLines(path + data_monster);
    }

    /// <summary>
    /// 넘겨받은 드랍아이템의 이름과 동일한 Line의 정보를 가져오는 매서드
    /// </summary>
    /// <param name="_itemName">CSV에 작성한 드랍아이템의 이름을 받는 매개변수</param>
    /// <returns></returns>
    public string[] GetDropItemDatas(string _itemName)
    {
        for(int i = 1; i < dropItemDatas.Length; i++)
        {
            string[] datas = dropItemDatas[i].Split(',');
            if (_itemName.Contains(datas[1]))
                return datas;
        }
        return null;
    }

    /// <summary>
    /// 넘겨받은 드랍아이템의 Index와 일치한 아이템의 정보를 가져오는 매서드
    /// </summary>
    /// <param name="_index">CSV에 작성한 드랍아이템의 Index를 그대로 받는 매개변수</param>
    /// <returns></returns>
    public string[] GetDropItemDatas(int _index)
    {
        for(int i = 1; i < dropItemDatas.Length; i++)
        {
            string[] datas = dropItemDatas[i].Split(',');
            if(datas[0].Contains(_index.ToString()) )
                return datas;
        }
        return null;
    }

    // 밑에내용 주석해제 필요
    // /// <summary>
    // /// 넘겨받은 몬스터의 Index와 일치한 몬스터의 정보를 가져오는 매서드
    // /// </summary>
    // /// <param name="_monsterIndex">CSV에 작성한 몬스터의 Index를 그대로 받는 매개변수</param>
    // /// <returns></returns>
    // public string[] GetMonsterDatas(int _monsterIndex)
    // {
    //     for(int i = 1; i < monsterDatas.Length; i++)
    //     {
    //         string[] datas = monsterDatas[i].Split(',');
    //         if(datas[0].Contains(_monsterIndex.ToString()) )
    //             return datas;
    //     }
    //     return null;
    // }
}
