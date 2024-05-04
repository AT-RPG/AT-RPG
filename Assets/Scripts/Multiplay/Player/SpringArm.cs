using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour
{
    public LayerMask crashMask;
    float curRotX, curRotY;
    public float rotSpeed = 10.0f;
    public float smoothRotSpeed = 5.0f;
    public Vector2 rotXRange = new Vector2(-60.0f, 80.0f);
    Quaternion targetRotX = Quaternion.identity;
    Quaternion targetRotY = Quaternion.identity;
    float targetDist;
    float camDist;
    Transform myCam = null;
    public Vector2 zoomRange = new Vector2(1, 8);
    public float zoomSpeed = 5.0f;

    [Header("Mouse Look")] 
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensivity = 2.0f;
    [SerializeField] private float mouseVerticalClamp = 90.0f;

    private float _verticalRotation;
    // Start is called before the first frame update
    void Start()
    {
        targetRotX = transform.localRotation;
        curRotX = transform.localRotation.eulerAngles.x;

        targetRotY = transform.parent.localRotation;
        curRotY = targetRotY.eulerAngles.y;

        myCam = GetComponentInChildren<Camera>().transform;
        camDist = targetDist = Mathf.Abs(myCam.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        // LookAtMouse();
        if(Input.GetMouseButton(1))
        {
            curRotX -= Input.GetAxis("Mouse Y") * rotSpeed;
            curRotX = Mathf.Clamp(curRotX, rotXRange.x, rotXRange.y);
            targetRotX = Quaternion.Euler(curRotX, 0, 0);

            curRotY += Input.GetAxis("Mouse X") * rotSpeed;
            targetRotY = Quaternion.Euler(0, curRotY, 0);
        }

        transform.localRotation = Quaternion.Slerp(transform.localRotation, 
            targetRotX, Time.deltaTime * smoothRotSpeed);
        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            targetRotY, Time.deltaTime * smoothRotSpeed);

        targetDist -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        targetDist = Mathf.Clamp(targetDist, zoomRange.x, zoomRange.y);

        float offSet = 0.5f;
        camDist = Mathf.Lerp(camDist, targetDist, Time.deltaTime);
        if(Physics.Raycast(new Ray(transform.position, -transform.forward),
            out RaycastHit hit, camDist + offSet, crashMask))
        {
            camDist = hit.distance - offSet;
        }

        myCam.localPosition = new Vector3(0, 0, -camDist);
    }

    // void LookAtMouse()
    // {
    //     curRotX = Input.GetAxis("Mouse X");
    //     curRotY = Input.GetAxis("Mouse Y");

    //     _verticalRotation += -curRotY * mouseSensivity;
    //     _verticalRotation = Mathf.Clamp(_verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
    //     playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
    //     transform.rotation *= Quaternion.Euler(0, curRotX * mouseSensivity, 0);
    // }
}
