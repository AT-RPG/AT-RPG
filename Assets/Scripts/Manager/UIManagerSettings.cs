using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "Scriptable Object/UIManager Setting")]
    public class UIManagerSettings : ScriptableObject
    {
        [Tooltip("화면을 어둡게 하는 효과를 가진 Game Object, " +
                 "'Fade' 컴포넌트와 'Image' 컴포넌트가 필요")]
        [SerializeField] public GameObject Fade;
    }

}
