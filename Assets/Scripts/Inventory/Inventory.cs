using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AT_RPG.Manager;
using System;
using System.Linq;

namespace AT_RPG
{
    /// <summary>
    /// 인벤토리 UI에 들어있는 모든 Slot의 정보를 가지고 있는 클래스
    /// </summary>
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance;
        public PlayerInventorySlot[] playerInventorySlots;
        public List<ItemSlot> itemSlots = new();
        public List<Item> items = new();
        
        private void Start()
        {
            Instance = this;
        }

        public void AddInventory(Item _item, int _amount)
        {
            for(int i = 0; i < playerInventorySlots.Length; i++)
            {
                ItemSlot slotInfo = itemSlots.FirstOrDefault(itemSlot => itemSlot.item != null && itemSlot.item.itemName == _item.itemName);

                if(slotInfo != null)
                {
                    AddItemInInventory(_item, _amount, slotInfo);
                    break;
                }
                else
                {
                    if(playerInventorySlots[i].curItemSlot == null)
                    {
                        GameObject obj = Resources.Load<GameObject>("Item");
                        GameObject objItem = Instantiate(obj, playerInventorySlots[i].transform);
                        ItemSlot itemSlot = objItem.GetComponent<ItemSlot>();
                        Image itemImage = objItem.GetComponentInChildren<Image>();

                        playerInventorySlots[i].curItemSlot = itemSlot;
                        itemSlot.item = _item;
                        itemSlot.SetParent(playerInventorySlots[i].transform);

                        itemSlot.itemCount.text = _amount.ToString();
                        AddItemInInventory(_item, _amount, itemSlot);
                        itemImage.sprite = itemSlot.item.itemSprite;
                        itemSlot.itemImage = itemImage;

                        itemSlots.Add(itemSlot);

                        break;
                    }
                }
            }
        }

        private void AddItemInInventory(Item _item, int _amount, ItemSlot _itemSlot)
        {
            switch(_item.myType)
            {
                case DropItem.HealPotion:
                GameManager.Player.AddHealPotion(_amount);
                _itemSlot.itemCount.text = GameManager.Player.HealPotion.ToString();
                break;
                case DropItem.MonsterPiece:
                GameManager.Player.AddMonsterPiece(_amount);
                _itemSlot.itemCount.text = GameManager.Player.MonsterPiece.ToString();
                break;
                case DropItem.Rock:
                GameManager.Player.AddRock(_amount);
                _itemSlot.itemCount.text = GameManager.Player.Rock.ToString();
                break;
                case DropItem.Wood:
                GameManager.Player.AddWood(_amount);
                _itemSlot.itemCount.text = GameManager.Player.Wood.ToString();
                break;
            }
        }
    }
}

