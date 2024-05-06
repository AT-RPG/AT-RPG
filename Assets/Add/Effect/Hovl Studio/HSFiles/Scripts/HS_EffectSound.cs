using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_EffectSound : MonoBehaviour
{
    private AudioClip clip;
    private AudioSource soundComponent;

    // void Start ()
    // {
    //     soundComponent = GetComponentInChildren<AudioSource>(true);
    //     clip = soundComponent.clip;
    // }

    private void OnEnable() 
    {
        if(soundComponent == null)
        {
            soundComponent = GetComponentInChildren<AudioSource>(true);
            clip = soundComponent.clip;
        }
        RepeatSound();
    }

    void RepeatSound()
    {
        soundComponent.PlayOneShot(clip);
    }
}
