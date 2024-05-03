using System;

namespace AT_RPG
{
    // Player와 Monster의 공통 Stat을 담아둠
    [Serializable]
    public struct BaseBattleStat
    {
        public int maxHP;
        public int attackPoint;
        public int defendPoint;
        public float moveSpeed;
        public float attackDeley;
        public float skillCooltime;
    }
}
