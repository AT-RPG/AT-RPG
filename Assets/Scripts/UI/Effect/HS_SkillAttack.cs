using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AT_RPG
{
    public class HS_SkillAttack : MonoBehaviour, ISkillDamageSet
    {
        private int myDamage;
        void OnEnable()
        {
            List<Collider> targetColliders = Physics.OverlapSphere(transform.position, 2.0f).ToList();
            targetColliders = targetColliders.Where(collider => collider.gameObject.layer == LayerMask.NameToLayer("Monster")).ToList();

            foreach (Collider collider in targetColliders)
            {
                ICharacterDamage cd = collider.GetComponent<ICharacterDamage>();
                cd?.TakeDamage(myDamage);
                Debug.Log("myDamage &&&&&&&&&&&&&&&&&&&&" + myDamage);
            }
        }

        public void SetDamage(int _damage)
        {
            myDamage = _damage;
        }
    }
}
