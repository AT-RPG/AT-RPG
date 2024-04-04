using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 현재 데이터를 저장하는 클래스
    /// </summary>
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

        /// <summary>
        /// 플레이어가 가지고 있는 골드를 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_coin">계산해줄 코인의 개수</param>
        public void AddPlayerCoin(int _coin)
        {
            playerCoin += _coin;
        }

        /// <summary>
        /// 플레이어가 가지고 있는 포션을 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_potion">계산해줄 포션의 개수</param>
        public void AddPlayerHealPotion(int _potion)
        {
            playerHealPotion += _potion;
        }

        /// <summary>
        /// 플레이어가 가지고 있는 몬스터 조각을 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_piece">계산해줄 몬스터 조각의 개수</param>
        public void AddPlayerMonsterPiece(int _piece)
        {
            playerMonsterPiece += _piece;
        }
    }
}
