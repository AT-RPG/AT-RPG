using Unity.Collections;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "MultiplayManagerSettings", menuName = "ScriptableObject/MultiplayManager Setting")]
    public partial class MultiplayManagerSetting : ScriptableObject
    {
        [ReadOnly] public readonly string AuthenticationDataPath = Application.streamingAssetsPath;

        [Tooltip("포톤 API 랩핑 인스턴스")]
        public ResourceReference<GameObject> MultiplayNetworkRunnerPrefab;
    }
}