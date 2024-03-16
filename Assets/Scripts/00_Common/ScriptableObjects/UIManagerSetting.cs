using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "ScriptableObject/UIManager Setting")]
    public class UIManagerSetting : ScriptableObject
    {
        [Tooltip("캔버스 스칼라 설정")]
        public CanvasScaler                     canvasScalerSetting;


        [Header("전역 UI 프리팹")]


        [Tooltip("게임 메뉴 팝업 프리팹")]
        public ResourceReference<GameObject>    gameMenuPopupPrefab;

        [Tooltip("팝업을 관리하는 캔버스 인스턴스 프리팹")]
        public ResourceReference<GameObject>    popupCanvasPrefab;

        [Tooltip("로그 팝업 프리팹")]
        public ResourceReference<GameObject>    logPopupPrefab;
    }
}
