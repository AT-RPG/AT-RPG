using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Info : MonoBehaviour
{
  
    struct m_Info  //������ ���� ����ü�� ����
    {
        string m_Name; //�̸�
        float m_Health; //ü��
        float m_MoveSpeed; //�̵��ӵ�
    }
    /*  ---------����ü�� Ŭ�����߿� �����Ұ�----------
    class M_Info //������ ���� Ŭ������ ���� 
    {
        string m_Name;
        float m_Health; 
        float m_MoveSpeed;
    }
    */

    struct m_BattleInfo //������ ����
    {
        float m_Atk; //���ݷ�
        float m_Rpm; //���ݼӵ�
        float m_Range; //��Ÿ�
    }

    /*-������ �⺻���� ���뱸���� �����Ұ�
    struct m_Skill //������ ��ų 
    {
        float m_Cooldown; //��Ÿ��
                          //   float m_UseMana; //��� ����-���Ϳ��� ���������� �ο����� ����
    }
    */

}
