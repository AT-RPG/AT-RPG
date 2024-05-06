using System.Collections;
using System.Collections.Generic;
using AT_RPG.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    public class InventoryUIController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI goldText;

        private void OnEnable() 
        {
            goldText.text = GameManager.Player.Gold.ToString();
        }
    }
}

