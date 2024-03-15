using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPorperty : MonoBehaviour
{
    Animator _anim = null; //애니메이터 변수선언

    public Animator monsterAnim
    {
        get
        {//ㅌㅌㅌㅌㅌㄸ
            if (_anim == null) //애니메이터가 널값인 경우
            {
                _anim = GetComponent<Animator>(); //현재게임오브젝트의 애니메이터 컴포먼트 탐색
                if (_anim == null) //현재게임오브젝트에 없을경우
                {
                    _anim = GetComponentInChildren<Animator>(); //자식오브젝트에서 애니메이터 컴포넌트 탐색
                }
            }
            return _anim; //찾은 애니메이터 컴포넌트를 _anim에 담은후에 반환한다.
        }
    }
}
