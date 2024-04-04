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
        Wood
    }

    /// <summary>
    /// CSV파일에 DropItem이 가지고 있는 Column을 보기쉽게 Enum으로 정리
    /// </summary>
    public enum DropItemColumn
    {
        Index,
        Name,
        PriceBuy,
        PriceSell,
        MaxAmount,
        DropRate
    }

    public enum DropType
    {
        Destructible = 2,
        Monster = 3
    }
}
