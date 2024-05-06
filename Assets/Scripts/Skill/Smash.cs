using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class Smash : Skill
    {
        private int skillDamage = 20;
        private int otherDamage;
        Coroutine skill;
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Transform skillPoint;
        [SerializeField] private Transform startPoint;
        [SerializeField] private GameObject skillEffect;

        private void OnEnable() 
        {
            skill = StartCoroutine(UsingSmash());
        }

        IEnumerator UsingSmash()
        {
            trailRenderer.gameObject.SetActive(true);

            yield return new WaitForSeconds(1.0f);
            trailRenderer.gameObject.SetActive(false);
            skillPoint.position = startPoint.position;

            Transform skillEffectTransform = skillEffect.GetComponent<Transform>();
            float yRotation = startPoint.eulerAngles.y;
            skillEffectTransform.eulerAngles = new Vector3(skillEffectTransform.eulerAngles.x, yRotation, skillEffectTransform.eulerAngles.z);
            
            skillEffect.SetActive(true);

            ISkillDamageSet[] sets = skillEffect.GetComponentsInChildren<ISkillDamageSet>();
            foreach (var skill in sets)
            {
                skill.SetDamage(otherDamage + skillDamage);
            }
            
            yield return new WaitForSeconds(2.8f);
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
                // StartCoroutine(UsingSmash());
                gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"{skillName}은 쿨타임중입니다. {skillCurCooltime}초 후에 다시 사용가능합니다.");
            }
        }

        private void OnDisable() 
        {
            StopCoroutine(skill);
        }
    }
}
