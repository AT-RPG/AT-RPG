using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
#if UNITY_EDITOR
    public class FileGenerator : MonoBehaviour
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
            BuildPipeline.BuildAssetBundles(
                setting.AssetBundlesSavePath,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            // 함수 종료 로그
            Debug.Log($"에셋 번들 생성 완료!, 생성 경로 : {setting.AssetBundlesSavePath}");
        }
    }
#endif

}

