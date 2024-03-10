using System.IO;
using UnityEditor;
using UnityEngine;

namespace AT_RPG
{
#if UNITY_EDITOR
    public class AssetBundleGenerator : MonoBehaviour
    {
        [MenuItem("BuildAssetBundles/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            // 에셋 번들 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // 스트리밍 폴더 만들기
            if (!Directory.Exists(setting.AssetBundleSavePath))
            {
                Directory.CreateDirectory(setting.AssetBundleSavePath);
            }

            // 에셋 번들 빌드
            BuildPipeline.BuildAssetBundles(setting.AssetBundleSavePath, BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            // 함수 종료 로그
            Debug.Log($"에셋 번들 생성 완료!, 생성 경로 : {setting.AssetBundleSavePath}");
        }
    }

#endif

}