using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCinter : MonoBehaviour
{
    public LayerMask layer;
    public bool canInter = false; // 상호작용가능 여부
    public GameObject npcInter; //상호작용 UI 변수
    public GameObject choicePanel; // 상호작용 시 출력되는 패널
    public GameObject chatPanel; // 대화하기 패널
    public GameObject storePanel; // 상점 패널
    public Camera zoomCamera; // 상호작용 시 줌 되는 카메라
    public float newFov;

    public void OnTriggerStay(Collider other)
    {
        if (((1 << other.gameObject.layer) & layer) != 0)
        {
            if (!choicePanel.activeSelf)
            {
                npcInter.SetActive(true); // 대화하기 UI 활성화
            }
            else
            {
                npcInter.SetActive(false);
            }
            if (canInter == false && Input.GetKeyDown(KeyCode.F))
            {
                StartInteraction();
                ZoomCam();
            }
            // 상호작용 중일 때 상호작용 중지
            else if (canInter == true && Input.GetKeyDown(KeyCode.F))
            {
                EndInteraction();
                ReturnCam();
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        npcInter.SetActive(false);
    }
    // 상호작용 시작
    public void StartInteraction()
    {
        canInter = true;
        Debug.Log("NPC와 상호작용 시작");
        //npcInter.SetActive(false);
        choicePanel.SetActive(true);
        // 여기에 상호작용 UI 표시 등의 코드 추가
    }

    // 상호작용 종료
    public void EndInteraction()
    {
        canInter = false;
        //npcInter.SetActive(true);
        choicePanel.SetActive(false);
        Debug.Log("NPC와 상호작용 종료");
        // 여기에 상호작용 UI 숨기기 등의 코드 추가
    }
    public void ClickChatButton()
    {
        chatPanel.SetActive(true);
        choicePanel.SetActive(false);
        npcInter.SetActive(false);
    }
    public void ClickStoreButton()
    {
        storePanel.SetActive(true);
        choicePanel.SetActive(false);
        npcInter.SetActive(false);
    }
    public void ClickEndButton()
    {
        EndInteraction();
        ReturnCam();
    }

    public void ZoomCam()
    {
        if (zoomCamera == null)
        {
            zoomCamera = GetComponent<Camera>();
        }
        Vector3 direction = transform.position - zoomCamera.transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        zoomCamera.transform.rotation = rotation;
        ChangeFov(newFov);
        zoomCamera.depth = zoomCamera.depth + 3;
    }
    public void ChangeFov(float fov)
    {
        zoomCamera.fieldOfView = fov;
    }
    public void ReturnCam()
    {
        ChangeFov(60.0f);
        zoomCamera.depth = zoomCamera.depth - 3;
    }

        
}