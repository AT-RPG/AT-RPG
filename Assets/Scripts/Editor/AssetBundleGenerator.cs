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
            // 스트리밍 폴더 만들기
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }

            // 에셋 번들 빌드
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            // 함수 종료 로그
            Debug.Log("BuildAssetBundles() is done.");
        }
    }

#endif

}