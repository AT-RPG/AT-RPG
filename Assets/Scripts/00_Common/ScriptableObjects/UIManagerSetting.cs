using UnityEngine;
using UnityEngine.UI;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "UIManagerSettings", menuName = "ScriptableObject/UIManager Setting")]
    public class UIManagerSetting : ScriptableObject
    {
        [Tooltip("캔버스 스칼라 설정")]
        public CanvasScaler canvasScalerSettings;
    }
}
