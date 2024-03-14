using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 :                                 <br/>
    /// + 팝업을 생성하는 버튼에서 사용되는 클래스 <br/>
    /// </summary>
    public class PopupButton : MonoBehaviour
    {
        [Tooltip("인스턴싱할 팝업 프리팹")]
        [SerializeField] protected ResourceReference<GameObject> popupPrefab;

        /// <summary>
        /// 팝업을 생성 및 초기화합니다.
        /// </summary>
        /// <param name="popupCanvas">팝업UI 관리 캔버스</param>
        public void OnInstantiatePopupAt(PopupCanvas popupCanvas)
        {
            GameObject popupInstance 
                = Instantiate(popupPrefab.Resource, popupCanvas.Root.transform);
            Popup popup = popupInstance.GetComponent<Popup>();
            popup.PopupCanvas = popupCanvas;
        }
    }
}
