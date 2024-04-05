using UnityEngine;
using UnityEngine.AddressableAssets;
using AT_RPG.Manager;

namespace AT_RPG
{
    /// <summary>
    /// <see cref="AssetReference"/>를 <see cref="ResourceManager"/>의 기능과 함께 래핑하는 클래스<br/>
    /// </summary>
    [System.Serializable]
    public class AssetReferenceResource<T> : AssetReference where T : Object
    {
        // 리소스 매니저에서 캐시된 리소스를 가져옵니다.
        public T Resource => ResourceManager.Get<T>(m_AssetGUID);

        public AssetReferenceResource(string guid) : base(guid) { }
        public AssetReferenceResource(AssetReference assetReference) : base(assetReference.AssetGUID) { }

        // UnityEngine.Object.Instantiate에서 prefab 대신에 사용
        public static implicit operator T(AssetReferenceResource<T> assetReferenceResource) => assetReferenceResource.Resource;

        // AT_RPG.ResourceManager.LoadAssetAsync에서 key 대신에 사용
        public static implicit operator string(AssetReferenceResource<T> assetReferenceResource) => assetReferenceResource.AssetGUID;
    }
}