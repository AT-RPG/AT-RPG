using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item", order = int.MaxValue)]
    public class Item : ScriptableObject
    {
        public string itemName;
        public DropItem myType;
        public int priceBuy;
        public int priceSell;
        public int dropRate;
        public Sprite itemSprite;
        public GameObject itemPrefab;
    }
}

