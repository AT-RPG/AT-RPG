using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCinter : MonoBehaviour
{
    public KeyCode interKey = KeyCode.F; // 상호작용 키
    public float interDistance = 3f; // 상호작용 가능 거리
    public float playerDistance; // 플레이어 거리
    public GameObject player; // 플레이어 오브젝트
    public bool canInter = false; // 상호작용가능 여부
    public PlayerController playerMove; // 플레이어 참조 변수
    public GameObject npcInter; //상호작용 UI 변수
    public GameObject ChatInter; // NPC 대화 변수
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // 태그가 플레이어인 오브젝트 찾기
        playerMove = player.GetComponent<PlayerController>(); // 플레이어 컴포넌트 참조
    }

    void Update()
    {
        // 플레이어와 NPC 사이의 거리 계산
        playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (playerDistance <= interDistance) // 일정거리가 되면 
        {
            if(!ChatInter.activeSelf) npcInter.SetActive(true); // 대화하기 UI 활성화
            if (canInter == false && Input.GetKeyDown(interKey))
            {
                StopMove();
                StartInteraction();
            }
            // 상호작용 중일 때 상호작용 중지
            else if(canInter == true && Input.GetKeyDown(interKey) || Input.GetKey(KeyCode.Escape))
            {
                EndInteraction();
                ResumeMove();
            }
        }
        else
        {
            npcInter.SetActive(false); // 대화하기 Ui비활성화
        }
        // 상호작용 가능한 거리 내에 플레이어가 있고, 특정 키를 눌렀을 때 상호작용 시작

        if (canInter == false && playerDistance <= interDistance && Input.GetMouseButtonDown(0))
        {
            // 마우스 클릭 지점에서 Ray를 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast로 NPC 충돌 확인
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // NPC와 충돌했는지 확인
                if (hit.collider.CompareTag("NPC"))
                {
                    // NPC와의 거리가 상호작용 가능한 거리 이내인지 확인
                    if (Vector3.Distance(transform.position, hit.collider.transform.position) <= interDistance)
                    {
                        StopMove();
                        StartInteraction();
                    }
                }
            }
        }
    }

    // 상호작용 시작
    public void StartInteraction()
    {
        canInter = true;
        npcInter.SetActive(false);
        ChatInter.SetActive(true);
        Debug.Log("NPC와 상호작용 시작");
        // 여기에 상호작용 UI 표시 등의 코드 추가
    }

    // 상호작용 종료
    public void EndInteraction()
    {
        canInter = false;
        npcInter.SetActive(true);
        ChatInter.SetActive(false);
        Debug.Log("NPC와 상호작용 종료");
        // 여기에 상호작용 UI 숨기기 등의 코드 추가
    }
    public void StopMove()
    {
        if (playerMove != null)
        {
            playerMove.enabled = false; // 플레이어 이동 스크립트를 비활성화
        }
    }
    public void ResumeMove()
    {
        if (playerMove != null)
        {
            playerMove.enabled = true; // 플레이어 이동 스크립트를 활성화
        }
    }
}