using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR

public class TestBuildAssetBundles : MonoBehaviour
{
    [MenuItem("TestBuildAssetBundles/Build AssetBundles")]
    public static void BuildAssetBundles()
    {
        // 스트리밍 폴더 만들기
        if (!Directory.Exists(TestResourceManager.AssetBundleBuildDir))
        {
            Directory.CreateDirectory(TestResourceManager.AssetBundleBuildDir);
        }

        // 에셋 번들 빌드
        BuildPipeline.BuildAssetBundles(TestResourceManager.AssetBundleBuildDir, BuildAssetBundleOptions.None,
            EditorUserBuildSettings.activeBuildTarget);

        // 함수 종료 로그
        Debug.Log("BuildAssetBundles() is done.");
    }
}

#endif
