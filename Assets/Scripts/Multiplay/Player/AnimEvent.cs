using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityEvent attackAct;
    public UnityEvent deadAct;
    public UnityEvent<bool> comboCheckAct;
    public void OnAttack()
    {
        attackAct?.Invoke();
    }

    public void OnDead()
    {
        deadAct?.Invoke();
    }

    public void ComboCheck(int v)
    {
        switch(v)
        {
            case 0:
                //콤보 확인
                comboCheckAct?.Invoke(true);
                break;
            case 1:
                //콤보 확인 취소
                comboCheckAct?.Invoke(false);
                break;
        }        
    }
}
