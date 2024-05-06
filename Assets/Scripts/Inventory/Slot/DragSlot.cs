using System.Collections;
using System.Collections.Generic;
using AT_RPG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public static DragSlot Instance;
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemCount;
    public ItemSlot itemSlot;

    void Start()
    {
        Instance = this;
    }

    public void SetDragSlotItemInfo(Image _itemImage, TextMeshProUGUI _itemCount)
    {
        itemImage.sprite = _itemImage.sprite;
        itemCount.text = _itemCount.text;
        SetColor(0.7f);
    }

    public void SetColor(float _alpha)
    {
        Color imageColor = itemImage.color;
        Color textColor = itemCount.color;
        imageColor.a = _alpha;
        textColor.a = _alpha;

        itemImage.color = imageColor;
        itemCount.color = textColor;
    }

    public void SetPosition(Vector3 _slotPosition)
    {
        transform.position = _slotPosition;
    }
}
