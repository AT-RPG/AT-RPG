namespace AT_RPG
{
    /// <summary>
    /// DropItem을 쉽게 알아보게 해주는 Enum
    /// </summary>
    public enum DropItem
    {
        Gold,
        HealPotion,
        MonsterPiece,
        Rock,
        Wood,
        Eqiupment
    }

    /// <summary>
    /// 몬스터와 파괴가능 오브젝트에서 DropItem을 쉽게 구분하기 위해 Enum으로 정리
    /// </summary>
    public enum DropType
    {
        /// <summary>
        /// 파괴가능한 오브젝트(Destructable Object)의 타입
        /// </summary>
        Destructible = 2,
        
        /// <summary>
        /// 몬스터 오브젝트의 타입
        /// </summary>
        Monster = 3
    }
}
