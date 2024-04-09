using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DropItemData", menuName = "ScriptableObject/DropItem", order = int.MaxValue)]
    public class DropItemData : ScriptableObject
    {
        public List<DropItemStat> dropItemStat = new List<DropItemStat>();
    }

    [System.Serializable]
    public class DropItemStat
    {
        public int index;
        public string itemName;
        public int priceBuy;
        public int priceSell;
        public int maxAmount;
        public int dropRate;
    }
}

