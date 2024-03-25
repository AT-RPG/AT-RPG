using AT_RPG.Manager;
using TMPro;
using UnityEngine;

namespace AT_RPG
{
    public class TitleSceneEventHandler : MonoBehaviour
    {
        /// <summary>
        /// 팝업을 생성합니다.
        /// </summary>
        public void OnInstantiatePopup(GameObject popupPrefab)
        {
            UIManager.InstantiatePopup(popupPrefab, PopupRenderMode.Hide);
        }
    }
}