using UnityEngine;
using UnityEditor;
using System.IO;

namespace AT_RPG
{
    public class CSVtoSO
    {
        public static readonly string path = Application.dataPath + "/Resources/CSVData/";
        [MenuItem("Generators/DropItem")]
        public static void GenerateDropItem()
        {
            string[] csvText = File.ReadAllLines(path + "JJappalWorld - DropItemData.csv");
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

            // foreach(var text in csvText)
            // {
            //     string[] stats = text.Split(',');

            //     DropItemStat dropItem = new()
            //     {
            //         index = int.Parse(stats[0]),
            //         itemName = stats[1],
            //         priceBuy = int.Parse(stats[2]),
            //         priceSell = int.Parse(stats[3]),
            //         maxAmount = int.Parse(stats[4]),
            //         dropRate = int.Parse(stats[5])
            //     };
            // }
            AssetDatabase.CreateAsset(dropItemData, "Assets/Scripts/00_Common/ScriptableObjects/Item/DropItem.asset");
            AssetDatabase.SaveAssets();
        }
    }
}