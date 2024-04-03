using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "MultiplayManagerSettings", menuName = "ScriptableObject/MultiplayManager Setting")]
    public partial class MultiplayManagerSettings : ScriptableObject
    {
        [Tooltip("포톤 API 랩핑 인스턴스")]
        public GameObject MultiplayNetworkRunnerPrefab;
    }
}