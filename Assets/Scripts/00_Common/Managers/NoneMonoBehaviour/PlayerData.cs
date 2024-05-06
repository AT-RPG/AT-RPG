using System;
using System.Collections;
using System.Collections.Generic;
using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 플레이어의 현재 데이터를 저장하는 클래스
    /// </summary>
    public class PlayerData
    {
        private int level;
        public int Level { get => level; }
        private int gold;
        public int Gold { get => gold; }
        private int healPotion;
        public int HealPotion { get => healPotion; }
        private int monsterPiece;
        public int MonsterPiece { get => monsterPiece; }
        private int rock;
        public int Rock { get => rock; }
        private int wood;
        public int Wood { get => wood; }

        /// <summary>
        /// 플레이어가 가지고 있는 골드를 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_coin">계산해줄 코인의 개수</param>
        public void AddGold(int _gold)
        {
            gold += _gold;
            Debug.Log("gold  ######" + gold);
        }

        /// <summary>
        /// 플레이어가 가지고 있는 포션을 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_potion">계산해줄 포션의 개수</param>
        public void AddHealPotion(int _potion)
        {
            healPotion += _potion;
            GameManager.Event.ChangePotionEvent?.Invoke();
            Debug.Log("healPotion  %%%%%" + healPotion);
        }

        /// <summary>
        /// 플레이어가 가지고 있는 몬스터 조각을 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_piece">계산해줄 몬스터 조각의 개수</param>
        public void AddMonsterPiece(int _piece)
        {
            monsterPiece += _piece;
            Debug.Log("monsterPiece  $$$$$$" + monsterPiece);
        }

        /// <summary>
        /// 플레이어가 가지고 있는 돌을 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_piece">계산해줄 몬스터 조각의 개수</param>
        public void AddRock(int _rock)
        {
            rock += _rock;
            Debug.Log("rock  $$$$$$" + rock);
        }

        /// <summary>
        /// 플레이어가 가지고 있는 나무를 더하거나 빼주는 함수
        /// </summary>
        /// <param name="_piece">계산해줄 몬스터 조각의 개수</param>
        public void AddWood(int _wood)
        {
            wood += _wood;
            Debug.Log("wood  $$$$$$" + wood);
        }
    }
}
