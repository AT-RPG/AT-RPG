using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public interface ISkillDamageSet
    {
        void SetDamage(int _playerDamage);
    }

    public abstract class Skill : MonoBehaviour
    {
        public string skillName;
        public float skillCooltime;
        public float skillCurCooltime;

        public void UpdateCooltime(float deltaTime)
        {
            if (skillCurCooltime > 0.0f)
            {
                skillCurCooltime -= deltaTime;
            }
            else
            {
                skillCurCooltime = 0.0f;
            }
        }

        public bool CanUse()
        {
            return skillCurCooltime <= 0;
        }

        public abstract void UseSkill(int _damage);
    }
}

