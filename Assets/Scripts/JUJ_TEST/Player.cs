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
        rb.freezeRotation = true; // 회전을 Rigidbody에게 맡기지 않음
    }

    void Update()
    {
        // 이동 로직
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // 회전 로직
        float rotate = Input.GetAxis("Horizontal") * rotateSpeed;
        rb.angularVelocity = new Vector3(0, rotate, 0);


        // 점프 로직
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("1");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
