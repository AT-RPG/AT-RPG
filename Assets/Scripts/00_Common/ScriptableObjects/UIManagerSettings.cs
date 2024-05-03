using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "ScriptableObject/UIManager Setting")]
    public class UIManagerSettings : ScriptableObject
    {
        [Tooltip("캔버스 스칼라 설정")]
        public CanvasScaler                     canvasScalerSetting;


        [Header("전역 UI 프리팹")]

        [Tooltip("게임 메뉴 팝업 프리팹")]
        public AssetReferenceResource<GameObject> gameMenuPopupPrefab;

        [Tooltip("팝업을 관리하는 캔버스 인스턴스 프리팹")]
        public AssetReferenceResource<GameObject> popupCanvasPrefab;

        [Tooltip("로그 팝업")]
        public AssetReferenceResource<GameObject> logPopupPrefab;

        [Tooltip("로딩 팝업")]
        public AssetReferenceResource<GameObject> loadingPopupPrefab;
    }
}
