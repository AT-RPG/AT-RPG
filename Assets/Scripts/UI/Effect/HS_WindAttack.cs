using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class HS_WindAttack : MonoBehaviour, ISkillDamageSet
    {
        [SerializeField] private int myDamage;

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                ICharacterDamage cd = other.GetComponent<ICharacterDamage>();
                cd?.TakeDamage(myDamage);
                Debug.Log("myDamage &&&&&&&&&&&&&&&&&&&&" + myDamage);
                gameObject.SetActive(false);
            }
        }

        public void SetDamage(int _damage)
        {
            myDamage = _damage;
        }
    }
}

