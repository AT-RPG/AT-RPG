using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    /// <summary>
    /// 등록된 팝업을 생성하는 클래스입니다. <br/>
    /// NOTE :  인스펙터의 '<see cref="Button.onClick"/>'에 '<see cref="OnInstantiatePopup"/>'를 연결해서 사용해주세요.
    /// </summary>
    public class PopupButton : MonoBehaviour
    {
        [Tooltip("인스턴싱할 팝업 프리팹")]
        [SerializeField] protected ResourceReference<GameObject> popupPrefab;

        [Tooltip("팝업 랜더링 옵션")]
        [SerializeField] protected PopupRenderMode popupRenderMode;

        /// <summary>
        /// 팝업을 생성합니다.
        /// </summary>
        public void OnInstantiatePopup()
        {
            UIManager.InstantiatePopup(popupPrefab.Resource, popupRenderMode);
        }
    }
}
