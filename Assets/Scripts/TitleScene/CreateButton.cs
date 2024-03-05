using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateButton : MonoBehaviour
{
    [SerializeField] private GameObject mapButton;

    /// <summary>
    ///  mapButton을 생성
    /// </summary>
    /// <param name="scrollViewContents">mapButton이 생성될 RectTransform 인스턴스</param>
    public void OnButtonClick(GameObject scrollViewContents)
    {
        if (!mapButton)
        {
            Debug.Log($"{nameof(mapButton)}이 설정X");
        }

        Instantiate(mapButton, scrollViewContents.transform);
    }
}
