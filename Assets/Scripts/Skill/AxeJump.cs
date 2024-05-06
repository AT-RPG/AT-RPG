using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class AxeJump : Skill
    {
        private int skillDamage = 10;
        private int otherDamage;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Transform skillPoint;
        [SerializeField] private GameObject skillEffect;

        private void OnEnable() 
        {
            StartCoroutine(UsingSmash());
        }

        IEnumerator UsingSmash()
        {
            trailRenderer.gameObject.SetActive(true);
            
            yield return new WaitForSeconds(2.0f);
            trailRenderer.gameObject.SetActive(false);
            skillPoint.position = transform.position;
            skillEffect.SetActive(true);
            
            ISkillDamageSet set = skillEffect.GetComponent<ISkillDamageSet>();
            set.SetDamage(otherDamage + skillDamage);

            ParticleSystem skillPS = skillEffect.GetComponent<ParticleSystem>();

            yield return new WaitForSeconds(skillPS.main.duration - 0.2f);
            skillEffect.SetActive(false);
            gameObject.SetActive(false);
        }

        public override void UseSkill(int _damage)
        {
            if (CanUse())
            {
                otherDamage = _damage;
                skillCurCooltime = skillCooltime;
                // 스킬 사용
                gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"{skillName}은 쿨타임중입니다. {skillCurCooltime}초 후에 다시 사용가능합니다.");
            }
        }
    }
}
