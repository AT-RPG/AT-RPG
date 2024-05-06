using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public interface ISetParent
    {
        void SetParent(Transform parent);
    }

    public class PlayerInventorySlot : MonoBehaviour, IDropHandler
    {
        public ItemSlot curItemSlot;

        public void OnDrop(PointerEventData eventData)
        {
            if(curItemSlot == null || DragSlot.Instance.itemSlot == null) return;
            
            if(curItemSlot != null)
            {
                curItemSlot.ChangeParent(DragSlot.Instance.itemSlot.myParent);
            }

            DragSlot.Instance.itemSlot.SetParent(gameObject.transform);
            curItemSlot = DragSlot.Instance.itemSlot;
        }

        public void ChangeItem (ItemSlot _slot)
        {
            curItemSlot = _slot;
        }
    }
}
