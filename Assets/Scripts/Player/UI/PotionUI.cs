using System.Collections;
using System.Collections.Generic;
using AT_RPG.Manager;
using TMPro;
using UnityEngine;

public class PotionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    // Start is called before the first frame update
    void Start()
    {
        ChangePotion();
        GameManager.Event.ChangePotionEvent += ChangePotion;
    }

    // Update is called once per frame
    void Update()
    {
        ChangePotion();
    }

    public void ChangePotion()
    {
        potionCountText.text = GameManager.Player.PlayerHealPotion.ToString();
    }
}
