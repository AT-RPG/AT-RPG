using Unity.Collections;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "MultiplayManagerSettings", menuName = "ScriptableObject/MultiplayManager Setting")]
    public partial class MultiplayManagerSetting : ScriptableObject
    {
        [ReadOnly] public readonly string AuthenticationDataPath = Application.streamingAssetsPath;

        [Tooltip("게임 메뉴 팝업 프리팹")]
        public ResourceReference<GameObject> multiplayLauncherPrefab;
    }
}