using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float rotateSpeed = 2f;
    public float jumpForce = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // ȸ���� Rigidbody���� �ñ��� ����
    }

    void Update()
    {
        // �̵� ����
        float horizontal = Input.GetAxis("MouseX");
        float vertical = Input.GetAxis("MouseY");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // ȸ�� ����
        float rotate = Input.GetAxis("MouseX") * rotateSpeed;
        rb.angularVelocity = new Vector3(0, rotate, 0);


        // ���� ����
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("1");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ���� ������ ���� ��
        if (other.CompareTag("SpawnPoint"))
        {
            // �ٸ� ���� �������� �̵�
            MoveToNewSpawnPoint(other.transform);
        }
    }

    public void MoveToNewSpawnPoint(Transform newSpawnPoint)
    {
        // �÷��̾ ���ο� ���� �������� �̵�
        transform.position = newSpawnPoint.position;
        transform.rotation = newSpawnPoint.rotation;
    }
}
