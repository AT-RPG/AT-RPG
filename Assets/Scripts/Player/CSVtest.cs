using UnityEngine;
using AT_RPG;

public class CSVtest : MonoBehaviour
{
    public DropItemData[] dropItems;
    public string[] dropItemDatas;
    public string[] goldData;
    public string[] potionData;

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

    public string[] GetDropItemDatas(int _index)
    {
        for(int i = 1; i < dropItemDatas.Length; i++)
        {
            string[] datas = dropItemDatas[i].Split(',');
            if( datas[0].Contains(_index.ToString()) )
                return datas;
        }
        return null;
    }

    // public void SettingItemData(DropItem drop)
    // {
    //     int result;

    //     dropItems[(int)drop].index = int.Parse(goldData[(int)DropItemColumn.Index]);
    //     dropItems[(int)drop].itemName = goldData[(int)DropItemColumn.Name];

    //     bool success = int.TryParse(goldData[(int)DropItemColumn.PriceBuy], out result);
    //     dropItems[(int)drop].priceBuy = success ? result : null;

    //     success = int.TryParse(goldData[(int)DropItemColumn.PriceSell], out result);
    //     dropItems[(int)drop].priceSell = success ? result : null;

    //     dropItems[(int)drop].maxAmount = int.Parse(goldData[(int)DropItemColumn.MaxAmount]);
    // }
    // Start is called before the first frame update
    void Start()
    {
        LoadAllResourcesData();

        goldData = GetDropItemDatas("Gold");
        // SettingItemData(DropItem.Gold);

        potionData = GetDropItemDatas(10001);
        // SettingItemData(DropItem.HealPotion);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
