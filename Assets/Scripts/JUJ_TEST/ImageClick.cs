using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageClick : MonoBehaviour
{
    public GameObject overlayImage; // 두 번째 이미지의 GameObject
    public Transform firstPosition; // 첫 번째 이미지의 위치

    private void Start()
    {
        // 시작 시 두 번째 이미지를 비활성화합니다.
        overlayImage.SetActive(false);
    }

    public void OnImageClicked()
    {
        // 이미지가 이미 활성화되었는지 확인합니다.
        if (!overlayImage.activeSelf)
        {
            // 클릭 이벤트가 발생하면 두 번째 이미지를 활성화합니다.
            overlayImage.SetActive(true);
            overlayImage.transform.position = firstPosition.position;
        }
        else
        {
            // 이미지가 이미 활성화되어 있다면 비활성화합니다.
            overlayImage.SetActive(false);
        }
    }
}