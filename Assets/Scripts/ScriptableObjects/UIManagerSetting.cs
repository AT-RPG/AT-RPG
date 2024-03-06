using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "ScriptableObject/UIManager Setting")]
    public class UIManagerSetting : ScriptableObject
    {
        // 화면을 점점 보이게 하는 효과 생성
        public GameObject ScreenFadeInInstance;

        // 화면을 점점 안보이게 하는 효과 생성
        public GameObject ScreenFadeOutInstance;
    }
}
