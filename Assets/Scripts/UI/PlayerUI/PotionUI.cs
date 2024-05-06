using System.Collections;
using System.Collections.Generic;
using AT_RPG.Manager;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    public class PotionUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI potionCountText;
        // Start is called before the first frame update
        void Start()
        {
            GameManager.Event.ChangePotionEvent += ChangePotion;
            ChangePotion();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void ChangePotion()
        {
            potionCountText.text = GameManager.Player.HealPotion.ToString();
        }
    }
}
