using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AT_RPG
{
    public class ItemSlot : Slot, IBeginDragHandler, IDragHandler, IEndDragHandler, ISetParent
    {
        public Item item;
        public Transform myParent { get; private set; }
        public TextMeshProUGUI itemCount;
        private Vector2 dragOffset;

        private void OnEnable() 
        {
            if(item.myType == DropItem.Eqiupment)
            {
                itemCount.gameObject.SetActive(false);
            }
            itemCount.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            DragSlot.Instance.itemSlot = this;
            DragSlot.Instance.SetPosition(eventData.position);
            DragSlot.Instance.SetDragSlotItemInfo(itemImage, itemCount);

            itemImage.raycastTarget = false;
            dragOffset = (Vector2)DragSlot.Instance.transform.position - eventData.position;
            myParent = transform.parent;
        }

        public void OnDrag(PointerEventData eventData)
        {
            DragSlot.Instance.transform.position = eventData.position + dragOffset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragSlot.Instance.SetColor(0);
            DragSlot.Instance.itemSlot = null;

            transform.localPosition = Vector3.zero;
            itemImage.raycastTarget = true;
            Debug.Log("OnEndDrag 발동");
        }

        public void SetParent(Transform parent)
        {
            myParent = parent;
            transform.SetParent(myParent);
            transform.localPosition = Vector3.zero;
        }

        public void ChangeParent(Transform parent)
        {
            SetParent(parent);
            myParent.GetComponent<PlayerInventorySlot>().ChangeItem(this);
        }
    }
}
