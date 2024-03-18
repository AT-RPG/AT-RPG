using UnityEngine;
using AT_RPG;
using AT_RPG.Manager;


public class PlayerController : CharacterProperty
{
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Rigidbody playerRigid;
    [SerializeField] private float moveSpeed = 3.0f;
    // [SerializeField] Transform myCam;

    // public LayerMask crashMask;

    // float targetDist;
    // float camDist;

    /// <summary>
    /// 입력 키 추가
    /// </summary>
    void Awake()
    {
        InputManager.AddKeyAction("Dodge", Dodge);
        InputManager.AddKeyAction("Jump", Jump);
        InputManager.AddKeyAction("Move Forward/Move Backward", Move);
        InputManager.AddKeyAction("Move Left/Move Right", Move);
        InputManager.AddKeyAction("Aim", LookAround);
    }
    // Start is called before the first frame update
    void Start()
    {
        // camDist = targetDist = Mathf.Abs(myCam.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// 플레이어의 이동
    /// </summary>
    private void Move(InputValue value)
    {
        if(myAnim.GetBool("isRolling")) return;

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        myAnim.SetBool("isMove", isMove);
        if(isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z);
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z);
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
        Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    /// <summary>
    /// 카메라 시선 처리
    /// </summary>
    private void LookAround(InputValue value)
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

    /// <summary>
    /// 회피
    /// </summary>
    private void Dodge(InputValue value)
    {
        if(myAnim.GetBool("isJumping")) return;

        myAnim.SetTrigger("Dodge");
        myAnim.SetBool("isRolling", true);
    }

    /// <summary>
    /// 점프
    /// </summary>
    private void Jump(InputValue value)
    {
        if(myAnim.GetBool("isRolling") || myAnim.GetBool("isJumping")) return;

        myAnim.SetTrigger("Jump");
        myAnim.SetBool("isJumping", true);
        playerRigid.AddForce(Vector3.up * 200.0f);
    }

    /// <summary>
    /// 지면을 감지하는 값을 받아 처리하는 함수
    /// </summary>
    public void SetFallState(bool isGround)
    {
        myAnim.SetBool("isGround", isGround);
    }

    /// <summary>
    /// 오브젝트 삭제시(ex:씬 이동) 추가했던 키 제거 (중복방지)
    /// </summary>
    private void OnDestroy() 
    {
        InputManager.RemoveKeyAction("Dodge", Dodge);
        InputManager.RemoveKeyAction("Jump", Jump);
        InputManager.RemoveKeyAction("Move Forward/Move Backward", Move);
        InputManager.RemoveKeyAction("Move Left/Move Right", Move);
        InputManager.RemoveKeyAction("Aim", LookAround);
    }
}

