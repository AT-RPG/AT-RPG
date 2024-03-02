using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "Scriptable Object/UIManager Setting")]
    public class UIManagerSettings : ScriptableObject
    {
        [Tooltip("화면을 점점 투명하게 하는 효과")]
        [SerializeField] public GameObject ScreenFadeIn;

        [Tooltip("화면을 점점 불투명하게 하는 효과")]
        [SerializeField] public GameObject ScreenFadeOut;
    }

}
