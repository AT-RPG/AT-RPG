using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterProperty
{
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    [SerializeField] Transform myCam;
    // public LayerMask crashMask;

    // float targetDist;
    // float camDist;

    // Start is called before the first frame update
    void Start()
    {
        // camDist = targetDist = Mathf.Abs(myCam.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();
        Move();
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            myAnim.SetTrigger("isDodge");
            // Dodge();
        }
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        myAnim.SetBool("isMove", isMove);
        if(isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z);
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z);
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 3.0f;
        }
        Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        float x = camAngle.x - mouseDelta.y;
        if(x < 180.0f)
        {
            x = Mathf.Clamp(x, -1f, 70.0f);
        }
        else
        {
            x = Mathf.Clamp(x, 335.0f, 361.0f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

        // float offSet = 0.5f;
        // camDist = Mathf.Lerp(camDist, targetDist, Time.deltaTime);
        // if(Physics.Raycast(new Ray(transform.position, -transform.forward),
        //     out RaycastHit hit, camDist + offSet, crashMask))
        // {
        //     camDist = hit.distance - offSet;
        // }

        // myCam.localPosition = new Vector3(0, 0, -camDist);
    }

    private void Dodge()
    {
        myAnim.SetTrigger("isDodge");
    }
}
