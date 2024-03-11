using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

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

            // 함수 종료 로그
            Debug.Log($"에셋 번들 생성 완료!, 생성 경로 : {setting.AssetBundlesSavePath}");

            // 에셋 번들의 리소스중에 이름과 타입이 모두 같은(중복된) 리소스 검사
            CheckResourceIntegrity(setting.AssetBundlesSavePath, assetBundleManifest);
        }

        /// <summary>
        /// 에셋 번들에 등록되는 리소스에 중복이 있는지 확인
        /// </summary>
        /// <param name="assetBundleManifest">에셋 번들 빌드</param>
        private static void CheckResourceIntegrity(
           string assetBundleSavePath, AssetBundleManifest assetBundleManifest)
        {
            List<Action> errorLog = new List<Action>();

            // 타입, 리소스 이름을 저장
            Dictionary<string, Dictionary<string, UnityEngine.Object>> resources
                = new Dictionary<string, Dictionary<string, UnityEngine.Object>>();

            foreach (var assetBundleName in assetBundleManifest.GetAllAssetBundles())
            {
                // 리소스에 중복을 찾기
                AssetBundle assetBundle =
                    AssetBundle.LoadFromFile(Path.Combine(assetBundleSavePath, assetBundleName));
                foreach (var resource in assetBundle.LoadAllAssets())
                {
                    string resourceType = resource.GetType().Name;
                    string resourceName = resource.name;

                    // 타입 저장
                    if (!resources.ContainsKey(resourceType))
                    {
                        resources[resourceType] = new Dictionary<string, UnityEngine.Object>();
                    }

                    // 리소스 이름 저장
                    if (!resources.ContainsKey(resourceName))
                    {
                        resources[resourceType][resourceName] = null;
                    }
                    else
                    // 리소스 이름이 중복, = 리소스가 중복
                    {
                        Debug.LogError($"에셋 번들'{assetBundleName}' 의 리소스'{resourceName}'가 중복..." +
                                       $"리소스 중복을 해결 후, 다시 에셋 번들을 빌드해주세요.");
                    }
                }

                assetBundle.Unload(true);
            }
        }
    }
#endif

}

