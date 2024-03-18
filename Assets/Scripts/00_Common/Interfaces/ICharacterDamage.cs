using System;

namespace AT_RPG
{
    public interface ICharacterDamage
    {
        /// <summary>
        /// Player와 Monster사이의 상호 Damage주고받기
        /// </summary>
        public void TakeDamage(float dmg);
    }
}
