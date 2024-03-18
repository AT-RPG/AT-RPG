using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                                          <br/>
    /// + 맵 선택 팝업에서 맵을 새로 생성하는 버튼에 사용되는 클래스         <br/>
    /// </summary>
    public class CreateMapPopupButton : PopupButton
    {
        [Tooltip("맵 선택 팝업 인스턴스")]
        [SerializeField] private GameObject startGamePopupInstance;

        /// <summary>
        /// 팝업을 생성 및 초기화합니다.
        /// </summary>
        public void OnInstantiatePopup()
        {
            PopupCanvas popupCanvas 
                = startGamePopupInstance.GetComponent<PopupCanvas>();

            GameObject popupInstance
                = Instantiate(popupPrefab.Resource, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupCanvas = popupCanvas;
        }
    }

}