using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanMamager : MonoBehaviour
{
    public Transform newSpawnPoint;  // Inspector���� �ٸ� ���� ������ �Ҵ��� ����

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ���� ������ ������ ��
        if (other.CompareTag("Player"))
        {
            // �÷��̾ �ٸ� ���� �������� �̵�
            if (newSpawnPoint != null)
            {
                other.GetComponent<Player>().MoveToNewSpawnPoint(newSpawnPoint);
            }
        }
    }
}
