using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "ScriptableObject/UIManager Setting")]
    public class UIManagerSetting : ScriptableObject
    {
        [Tooltip("화면을 점점 보이게 하는 효과를 생성하는 인스턴스")]
        public GameObject ScreenFadeInInstance;

        [Tooltip("화면을 점점 안보이게 하는 효과 생성하는 인스턴스")]
        public GameObject ScreenFadeOutInstance;


        [Header("전역 캔버스 설정")]
        [Header("전역 설정용 CanvasScaler")]
        public CanvasScaler CanvasScalerSettingReference;
    }
}
