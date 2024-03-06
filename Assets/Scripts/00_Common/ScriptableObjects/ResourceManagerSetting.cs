using Unity.Collections;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "ResourceManagerSettings", menuName = "ScriptableObject/ResourceManager Setting")]
    public class ResourceManagerSetting : ScriptableObject
    {
        // 에셋 번들 저장 경로
        [ReadOnly] public readonly string AssetBundleSavePath = Application.streamingAssetsPath;

        // 전역 에셋 번들 네이밍
        [ReadOnly] public readonly string GlobalAssetBundleName = "Global";

        // 리소스 페이크 로딩 지속 시간
        // NOTE : 비동기 로딩에만 적용
        public float FakeLoadingDuration = 0.75f;
    }
}

