using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPorperty : MonoBehaviour
{
    Animator _anim = null; //�ִϸ����� ��������

    public Animator monsterAnim
    {
        get
        {
            if (_anim == null) //�ִϸ����Ͱ� �ΰ��� ���
            {
                _anim = GetComponent<Animator>(); //������ӿ�����Ʈ�� �ִϸ����� ������Ʈ Ž��
                if (_anim == null) //������ӿ�����Ʈ�� �������
                {
                    _anim = GetComponentInChildren<Animator>(); //�ڽĿ�����Ʈ���� �ִϸ����� ������Ʈ Ž��
                }
            }
            return _anim; //ã�� �ִϸ����� ������Ʈ�� _anim�� �����Ŀ� ��ȯ�Ѵ�.
        }
    }
}
