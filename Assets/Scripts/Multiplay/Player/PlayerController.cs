using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AT_RPG;
using AT_RPG.Manager;
using System.Linq;

/// <summary>
/// MainPlayer의 State를 조절해주는 클래스
/// </summary>
public class PlayerController : CommonBattle
{
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private Transform characterBody;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Rigidbody playerRigid;
    [SerializeField] private int curWeaponDamage = 0;
    [SerializeField] private Transform[] weaponAttackPoints;
    [SerializeField] private Transform myAttackPoint;
    [SerializeField] private Skill curSkill;
    [SerializeField] private InventoryUIController inventoryUIController;
    [SerializeField] private GameObject heal;
    private bool isComboCheck = false;
    bool isInventoryOn = false;
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
        InputManager.AddKeyAction("Attack/Fire", Attack);
        InputManager.AddKeyAction("UsePotion", UseHealPotion);
        InputManager.AddKeyAction("UseSkill", UseSkill);
        InputManager.AddKeyAction("Inventory", OnOffInventory);
    }
    // Start is called before the first frame update
    void Start()
    {
        base.Initialize();
        GameManager.Event.ChangeSkillSpriteEvent?.Invoke(null);
        // camDist = targetDist = Mathf.Abs(myCam.localPosition.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(curSkill != null)
        {
            curSkill.UpdateCooltime(Time.deltaTime);
            GameManager.Event.CheckSkillCooldownEvent?.Invoke((curSkill.skillCooltime - curSkill.skillCurCooltime) / curSkill.skillCooltime);
        }
    }

    /// <summary>
    /// 플레이어의 이동
    /// </summary>
    private void Move(InputValue value)
    {
        if(myAnim.GetBool("isRolling") || myAnim.GetBool("isAttacking") || myAnim.GetBool("isUsingPotion")) return;

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (isInventoryOn) // 인벤토리 켜져있을때는 정지
        {
            myAnim.SetBool("isMove", false);
            return;
        }

        bool isMove = moveInput.magnitude != 0;
        myAnim.SetBool("isMove", isMove);
        if(isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z);
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z);
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * baseBattleStat.moveSpeed;
        }
        // Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    /// <summary>
    /// 카메라 시선 처리
    /// </summary>
    private void LookAround(InputValue value)
    {
        if(isInventoryOn) return;

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
        if(myAnim.GetBool("isJumping") || myAnim.GetBool("isAttacking")) return;

        myAnim.SetTrigger("Dodge");
        myAnim.SetBool("isRolling", true);
    }

    /// <summary>
    /// 점프
    /// </summary>
    private void Jump(InputValue value)
    {
        if(myAnim.GetBool("isRolling") || myAnim.GetBool("isJumping") || myAnim.GetBool("isAttacking") || myAnim.GetBool("isUsingPotion")) return;

        myAnim.SetTrigger("Jump");
        myAnim.SetBool("isJumping", true);
        playerRigid.AddForce(Vector3.up * 200.0f);
    }

    /// <summary>
    /// 콤보공격 체크 ( 다른 스크립트에서 UnityEvent로 실행중 )
    /// </summary>
    public void ComboCheck(bool v)
    {
        if(v)
        {
            StartCoroutine(ComboChecking());
        }
        else
        {
            isComboCheck = false;
        }
    }

    /// <summary>
    /// 콤보 공격 마우스 입력값 체크하는 코루틴
    /// </summary>
    IEnumerator ComboChecking()
    {
        int clickCount = 0;
        isComboCheck = true;
        myAnim.SetBool("isComboAttack", true);
        while (isComboCheck)
        {
            if(Input.GetMouseButtonDown(0))
            {
                clickCount++;
            }
            yield return null;
        }

        if(clickCount == 0)
        {
            myAnim.SetBool("isComboAttack", false);
        }
    }

    /// <summary>
    /// 마우스 좌클릭 시 공격트리거 작동
    /// </summary>
    private void Attack(InputValue value)
    {
        if(!myAnim.GetBool("isAttacking"))
        {
            if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                myAnim.SetTrigger("NormalAttack");
            }
        }
    }

    /// <summary>
    /// 공격 시 지정해둔 AttackPoitn에서 Overlap하여 Monster Collider 검출 후 검출 된 Monster에게 대미지 전달
    /// </summary>
    public new void OnAttack()
    {
        List<Collider> targetColliders = Physics.OverlapSphere(myAttackPoint.position, 1.0f).ToList();
        targetColliders = targetColliders.Where(collider => collider.gameObject.layer != LayerMask.NameToLayer("Player")).ToList();

        foreach (Collider collider in targetColliders)
        {
            ICharacterDamage cd = collider.GetComponent<ICharacterDamage>();
            cd?.TakeDamage(baseBattleStat.attackPoint + curWeaponDamage);
        }
    }

    /// <summary>
    /// WeaponData변경시 카메라의 보이는 무기오브젝트 변경, 무기에 따라 AttackPoint 위치 변경
    /// </summary>
    /// <param name="curWeapon">변경되는 무기타입</param>
    public void ChangeWeaponInfo(WeaponData curWeapon)
    {
        myAnim.runtimeAnimatorController = curWeapon.AnimatorOverride;
        curWeaponDamage = curWeapon.Damage;
        Debug.Log(baseBattleStat.attackPoint + curWeaponDamage);

        switch(curWeapon.MyState)
        {
            case WeaponState.None:
            weapons[0].SetActive(false);
            weapons[1].SetActive(false);
            myAttackPoint = weaponAttackPoints[(int)WeaponState.None];
            curSkill = null;
            GameManager.Event.ChangeSkillSpriteEvent?.Invoke(null);
            break;
            case WeaponState.OneHand:
            weapons[0].SetActive(true);
            weapons[1].SetActive(false);
            myAttackPoint = weaponAttackPoints[(int)WeaponState.OneHand];
            curSkill = GetComponentInChildren<AxeJump>(true);
            GameManager.Event.ChangeSkillSpriteEvent?.Invoke(curWeapon.SkillIconSprite);
            break;
            case WeaponState.TwoHand:
            weapons[0].SetActive(false);
            weapons[1].SetActive(true);
            myAttackPoint = weaponAttackPoints[(int)WeaponState.TwoHand];
            curSkill = GetComponentInChildren<Smash>(true);
            GameManager.Event.ChangeSkillSpriteEvent?.Invoke(curWeapon.SkillIconSprite);
            break;
        }
    }

    /// <summary>
    /// 플레이어 사망시 사망효과 실행해줌
    /// </summary>
    public void DisApear()
    {
        StartCoroutine(DisApearing(1.0f));
    }

    /// <summary>
    /// 플레이어 사망 시 처리되는 효과를 작동시키는 코루틴
    /// </summary>
    /// <param name="delay">사망 후 대기시간</param>

    IEnumerator DisApearing(float delay)
    {
        yield return new WaitForSeconds(delay);

        float dist = 1.0f;
        while(dist > 0.0f)
        {
            float delta = 0.5f * Time.deltaTime;
            dist -= delta;
            transform.Rotate(Vector3.up * delta * 720.0f, Space.World);
            yield return null;
        }
        myAnim.gameObject.SetActive(false);
    }

    /// <summary>
    /// 지면을 감지하는 값을 받아 처리하는 함수
    /// </summary>
    public void SetFallState(bool isGround)
    {
        myAnim.SetBool("isGround", isGround);
    }

    // protected override void OnDead()
    // {
    //     base.OnDead();
    // }

    /// <summary>
    /// HealPotion을 사용했을 경우 체력은 채워주고 아이템의 개수는 줄여줌
    /// </summary>
    public void UseHealPotion(InputValue value)
    {
        if(!myAnim.GetBool("isGround") || myAnim.GetBool("isRolling") || myAnim.GetBool("isJumping") 
        || myAnim.GetBool("isAttacking") || myAnim.GetBool("isUsingPotion")) return;

        if(GameManager.Player.HealPotion <= 0)
        {
            Debug.Log("물약이 없습니다");
        }
        else
        {
            myAnim.SetBool("isUsingPotion",true);
            curHP += 40.0f;
            GameManager.Player.AddHealPotion(-1);
            heal.SetActive(true);
        }
    }

    /// <summary>
    /// 무기에 따라 변경된 스킬을 사용하게 해줌
    /// </summary>
    public void UseSkill(InputValue value)
    {
        if(curSkill == null) return;

        if(!myAnim.GetBool("isGround") || myAnim.GetBool("isRolling") || myAnim.GetBool("isJumping") 
        || myAnim.GetBool("isAttacking") || myAnim.GetBool("isUsingPotion")) return;

        if(curSkill.CanUse()) myAnim.SetTrigger("Skill");
        curSkill?.UseSkill(baseBattleStat.attackPoint + curWeaponDamage);
    }

    /// <summary>
    /// 인벤토리를 열고 닫아줌
    /// </summary>
    public void OnOffInventory(InputValue value)
    {
        inventoryUIController.gameObject.SetActive(!inventoryUIController.gameObject.activeSelf);
        if(inventoryUIController.gameObject.activeSelf)
        {
            isInventoryOn = true;
        }
        else
        {
            isInventoryOn = false;
        }
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
        InputManager.RemoveKeyAction("Attack/Fire", Attack);
        InputManager.RemoveKeyAction("UsePotion", UseHealPotion);
        InputManager.RemoveKeyAction("UseSkill", UseSkill);
        InputManager.RemoveKeyAction("Inventory", OnOffInventory);
    }
}

