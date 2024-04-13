using UnityEngine;
using UnityEditor;
using AT_RPG.Manager;

namespace AT_RPG
{
    public class CSVtoSO
    {
        [MenuItem("Generators/DropItem")]
        public static void GenerateDropItem()
        {
            string[] csvText = GameManager.LoadCSVData.DropItemDatas;
            DropItemData dropItemData = ScriptableObject.CreateInstance<DropItemData>();

            for(int i = 1; i < csvText.Length; i++)
            {
                string[] datas = csvText[i].Split(',');

                DropItemStat dropItem = new()
                {
                    index = int.Parse(datas[0]),
                    itemName = datas[1],
                    priceBuy = int.Parse(datas[2]),
                    priceSell = int.Parse(datas[3]),
                    maxAmount = int.Parse(datas[4]),
                    dropRate = int.Parse(datas[5])
                };

                dropItemData.dropItemStat.Add(dropItem);
            }
            
            AssetDatabase.CreateAsset(dropItemData, "Assets/Scripts/00_Common/ScriptableObjects/Item/DropItem.asset");
            AssetDatabase.SaveAssets();
        }
    }
}