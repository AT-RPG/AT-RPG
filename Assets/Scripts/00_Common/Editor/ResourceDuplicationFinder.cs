using UnityEditor;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace AT_RPG
{
    /// <summary>
    /// 리소스 폴더와 에셋 번들 리소스에 이름과 타입이 모두 겹치는 파일이 있는지 확인하는 클래스  <br/>
    /// + CAUTION : 현재 변경사항에 대해서만 에러를 로그합니다. 에러를 무시하면 추후에 이 리소스가 중복되었는지 확인할 방법이 없습니다!!! <br/>
    /// + TODO : 에러가 발생하면 RevertAction
    /// </summary>
    public class ResourceDuplicationFinder : AssetPostprocessor
    {
        /// <summary>
        /// Assets폴더에 변화가 생기면, 리소스의 중복을 확인합니다.   <br/>
        /// + 중복된 파일들에 대한 로그를 출력합니다.                <br/>
        /// </summary>
        public static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
        {
            Stopwatch stopwatch = new Stopwatch();
            bool isDuplicated = false;

            stopwatch.Start();
            {
                isDuplicated = FindDuplicationResources(importedAssets, deletedAssets, movedAssets, null);
            }
            stopwatch.Stop();

            if (isDuplicated)
            {
                UnityDebug.Log($"{nameof(ResourceDuplicationFinder)}.cs에서 리소스 중복성 감지." +
                               $" 소요시간 : {stopwatch.ElapsedMilliseconds}ms");
            }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 리소스들에서 중복을 찾습니다.
        /// </summary>
        private static bool FindDuplicationResources(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
        {
            bool isDuplicated = false;

            // 변경된 에셋 중, 변경 타입이 Imported인 에셋
            foreach (var importedAsset in importedAssets)
            {
                // 중복된 이름을 가진 GUID들을 획득
                string importedAssetName = String.GetFileName(importedAsset);
                List<string> duplicatedNameAssetGUIDs = AssetDatabase.FindAssets(importedAssetName).ToList();

                // 중복 조건 적용
                ApplyDuplicationSearchFilter(importedAsset, ref duplicatedNameAssetGUIDs);

                // 중복된 에셋이 있음?
                if (duplicatedNameAssetGUIDs.Count >= 1)
                {
                    LogDuplicationMsg(importedAssetName, duplicatedNameAssetGUIDs);
                    isDuplicated = true;
                }
            }

            // 변경된 에셋 중, 변경 타입이 Moved인 에셋
            foreach (var movedAsset in movedAssets)
            {
                // 중복된 이름을 가진 GUID들을 획득 (자기 자신은 제거)
                string movedAssetName = String.GetFileName(movedAsset);
                List<string> duplicatedNameAssetGUIDs = AssetDatabase.FindAssets(movedAssetName).ToList();

                // 중복 조건 적용
                ApplyDuplicationSearchFilter(movedAsset, ref duplicatedNameAssetGUIDs);

                // 중복된 에셋이 있음?
                if (duplicatedNameAssetGUIDs.Count >= 1)
                {
                    LogDuplicationMsg(movedAssetName, duplicatedNameAssetGUIDs);
                    isDuplicated = true;
                }
            }

            return isDuplicated;
        }

        /// <summary>
        /// 중복 에셋인지를 판단하는 조건을 적용합니다.    <br/>
        /// + 자기 자신은 제외                           <br/>
        /// + 에셋의 타입이 다른 경우도 제외              <br/>
        /// </summary>
        private static void ApplyDuplicationSearchFilter(
            string assetPath, ref List<string> duplicatedNameAssetGUIDs)
        {
            // 자기 자신 제거
            duplicatedNameAssetGUIDs.Remove(AssetDatabase.AssetPathToGUID(assetPath));

            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // 1. 타입이 같지 않은 경우 제거
            // 2. 리소스 폴더나 에셋 번들의 리소스가 아니면 제거
            string assetType = String.GetFileType(assetPath);
            duplicatedNameAssetGUIDs = duplicatedNameAssetGUIDs.Where(guid =>
            {
                string duplicatedNameAssetPath = AssetDatabase.GUIDToAssetPath(guid);

                return (String.GetFileType(duplicatedNameAssetPath) == assetType) &&
                       (String.ContainsString(duplicatedNameAssetPath, "Resources") ||
                        String.ContainsString(duplicatedNameAssetPath, setting.AssetBundlesSavePath));

            }).ToList();
        }

        /// <summary>
        /// 중복 에셋이 있음을 경고 + 중복 에셋의 경로를 로그합니다.
        /// </summary>
        private static void LogDuplicationMsg(string assetName, List<string> duplicatedNameAssetGUIDs)
        {
            string logMsg = $"{assetName}와 중복되는 리소스가 있습니다.";

            // 중복 에셋의 경로 출력
            int logIndex = 1;
            foreach (var duplicatedNameAssetGUID in duplicatedNameAssetGUIDs)
            {
                string duplicatedNameAssetPath = AssetDatabase.GUIDToAssetPath(duplicatedNameAssetGUID);
                logMsg += $"\n 중복 리소스 경로 {logIndex++} : {duplicatedNameAssetPath}";
            }

            if (duplicatedNameAssetGUIDs.Count >= 1)
            {
                UnityDebug.LogError(logMsg);
            }
        }
    }
}
