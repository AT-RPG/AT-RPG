using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class HS_FrontMover : MonoBehaviour 
    {
        public Transform pivot;
        public ParticleSystem effect;
        public float speed = 15f;
        public float drug = 1f;
        private float startSpeed = 0f;

        void OnEnable()
        {
            StartAgain();
            effect.Play();
            startSpeed = speed;
        }

        void StartAgain()
        {
            startSpeed = speed;
            transform.position = pivot.position;
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Monster"))
            {
                ICharacterDamage cd = other.gameObject.GetComponent<ICharacterDamage>();
            }
        }

        void Update()
        {
            startSpeed = startSpeed * drug;
            transform.position += transform.forward * (startSpeed * Time.deltaTime);
        }
    }
}
