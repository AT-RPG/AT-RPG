using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform player;  // ĳ������ Transform ����
    public float distance = 2.0f;  // ī�޶�� ĳ���� ������ �Ÿ�
    public float height = 1.5f;  // ī�޶��� ����
    public float rotationSpeed = 3.0f; // ī�޶� ȸ�� �ӵ�

    void Update()
    {
        // �÷��̾��� ��ġ�� �������� ī�޶��� ��ġ ����
        Vector3 newPosition = player.position - player.forward * distance;
        newPosition.y = player.position.y + height;  // ���� ����
        transform.position = newPosition;
        float mouseX = Input.GetAxis("MouseKey X");
        // ���� ȸ��
        transform.Rotate(Vector3.up, mouseX * rotationSpeed);

        // �÷��̾ �ٶ󺸵��� ī�޶� ȸ��
        transform.LookAt(player);
    }
}
