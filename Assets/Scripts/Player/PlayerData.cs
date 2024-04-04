using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class PlayerData
    {
        private int playerLevel;
        public int PlayerLevel { get => playerLevel; }
        private int playerCoin;
        public int PlayerCoin { get => playerCoin; }
        private int playerHealPotion;
        public int PlayerHealPotion { get => playerHealPotion; }
        private int playerMonsterPiece;
        public int PlayerMonsterPiece { get => playerMonsterPiece; }
        

        public void AddPlayerCoin(int _coin)
        {
            playerCoin += _coin;
            Debug.Log("playerCoin &&&&& " + playerCoin);
        }

        public void AddPlayerHealPotion(int _potion)
        {
            playerHealPotion += _potion;
            Debug.Log("playerHealPotion &&&&& " + playerHealPotion);
        }

        public void AddPlayerMonsterPiece(int _piece)
        {
            playerMonsterPiece += _piece;
            Debug.Log("playerMonsterPiece &&&&& " + playerMonsterPiece);
        }
    }
}
