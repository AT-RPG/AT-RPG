using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace AT_RPG
{
#if UNITY_EDITOR
    public class AssetBundlesGenerator : MonoBehaviour
    {
        /// <summary>
        /// 모든 에셋 번들 파일 생성
        /// </summary>
        [MenuItem("Generators/Build AssetBundles")]
        public static void BuildAssetBundles()
        {
            Stopwatch stopwatch = new Stopwatch();

            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            stopwatch.Start();
            {
                // 스트리밍 폴더 만들기
                if (!Directory.Exists(setting.AssetBundlesSavePath))
                {
                    Directory.CreateDirectory(setting.AssetBundlesSavePath);
                }

                // 에셋 번들 빌드
                AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(
                    setting.AssetBundlesSavePath,
                    BuildAssetBundleOptions.None,
                    EditorUserBuildSettings.activeBuildTarget);
            }
            stopwatch.Stop();

            // 함수 종료 로그
            UnityDebug.Log($"{nameof(AssetBundlesGenerator)}.cs에서 에셋 번들 업데이트. \n" +
                      $"에셋 번들 파일 위치 : " +
                      $"{setting.AssetBundlesSavePath} \n" +
                      $"소요시간 : " +
                      $"{stopwatch.ElapsedMilliseconds}ms \n");
        }
    }
#endif

}

