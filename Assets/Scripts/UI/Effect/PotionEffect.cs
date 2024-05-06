using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class PotionEffect : MonoBehaviour
    {
        private void OnEnable() 
        {
            float durantion = GetComponent<ParticleSystem>().main.duration;
            Invoke("ActiveOff", durantion);
        }

        private void ActiveOff()
        {
            gameObject.SetActive(false);
        }
    }
}
