using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
#endif

namespace AT_RPG
{
    /// <summary>
    /// 런타임에 씬 에셋을 사용할 수 있도록 해주는 클래스
    /// </summary>
    [System.Serializable]
    public class AssetReferenceScene : AssetReference
    {
        public string SceneName => ResourceManager.Get<Object>(AssetGUID).name;

        public AssetReferenceScene(string guid) : base(guid) { }

        public override bool ValidateAsset(Object obj)
        {
#if UNITY_EDITOR
            var type = obj.GetType();
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj)));

            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type) && entry != null;
#else
        return false;
#endif
        }

        public override bool ValidateAsset(string path)
        {
#if UNITY_EDITOR
            var type = UnityEditor.AssetDatabase.GetMainAssetTypeAtPath(path);
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(path));

            return typeof(UnityEditor.SceneAsset).IsAssignableFrom(type) && entry != null;
#else
        return false;
#endif
        }
    }
}