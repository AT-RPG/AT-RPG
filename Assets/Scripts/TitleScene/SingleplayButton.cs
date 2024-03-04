using UnityEngine;

public class SingleplayButton : MonoBehaviour
{
    [SerializeField] private GameObject     startGameMenuPopup;

    /// <summary>
    /// 게임 시작관련 팝업 메뉴를 생성합니다
    /// </summary>
    public void InstantiateStartGameMenuPopup(GameObject canvas)
    {
        Instantiate(startGameMenuPopup, canvas.transform);
    }
}
