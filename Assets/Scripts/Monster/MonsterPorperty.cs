using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPorperty : MonoBehaviour
{
    Animator _anim = null;

    public Animator monsterAnim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponent<Animator>();
                if (_anim == null)
                {
                    _anim = GetComponentInChildren<Animator>();
                }
            }
            return _anim;
        }
    }
}
