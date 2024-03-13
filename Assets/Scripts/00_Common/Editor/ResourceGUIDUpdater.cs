using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;

namespace AT_RPG
{
    /// <summary>
    /// 리소스폴더와 에셋 번들 리소스에 변화가 있는지 확인하는 클래스입니다. <br/>
    /// + 변화가 있는 경우, 리소스 GUID 매핑을 업데이트 합니다.
    /// </summary>
    public class ResourceGUIDUpdater : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromAssetPaths)
        {
            Stopwatch stopwatch = new Stopwatch();
            bool isDirty = false;
            bool isDuplicated = false;
            stopwatch.Start();
            {
                // 리소스 폴더나 에셋 번들 리소스에서 변경사항이 있음?
                isDirty = IsResourceDirty(importedAssets, deletedAssets, movedAssets);
                if (isDirty)
                {
                    // 리소스에 중복이 없음?
                    isDuplicated = IsResourceDuplicationExist(importedAssets, movedAssets);
                    if (!isDuplicated)
                    {
                        // GUID 업데이트
                        ResourceGUIDMapFileGenerator.BuildResourceGUIDMapFile();
                        ResourceGUID.LoadResourceGUIDMap();
                    }
                }
            }
            stopwatch.Stop();

            if (isDirty && !isDuplicated)
            {
                UnityDebug.Log($"{nameof(ResourceGUIDUpdater)}.cs에서 리소스 GUID 매핑 업데이트. \n" +
                               $"리소스 GUID 매핑 캐시 위치 : " +
                               $"{nameof(ResourceGUID)}.{nameof(ResourceGUID.ResourceGUIDMapCache)} \n" +
                               $"소요시간 : " +
                               $"{stopwatch.ElapsedMilliseconds}ms \n");
            }
        }

        /// <summary>
        /// 리소스 폴더나 에셋 번들 리소스에 업데이트가 일어났는지 확인합니다.
        /// </summary>
        private static bool IsResourceDirty(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets)
        {
            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // 변경사항이 Imported인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = importedAssets.Where(assetPath =>
                {
                    return  String.GetFileType(assetPath) != "" &&
                           (String.ContainsString(assetPath, "Resources") ||
                            String.ContainsString(assetPath, setting.AssetBundlesSavePath));
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            // 변경사항이 deleted인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = deletedAssets.Where(assetPath =>
                {
                    return  String.GetFileType(assetPath) != "" &&
                           (String.ContainsString(assetPath, "Resources") ||
                            String.ContainsString(assetPath, setting.AssetBundlesSavePath));
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            // 변경사항이 moved인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = movedAssets.Where(assetPath =>
                {
                    return  String.GetFileType(assetPath) != "" &&
                           (String.ContainsString(assetPath, "Resources") ||
                            String.ContainsString(assetPath, setting.AssetBundlesSavePath));
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 리소스들에서 중복을 찾습니다.
        /// </summary>
        public static bool IsResourceDuplicationExist(
        string[] importedAssets,
        string[] movedAssets)
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
        public static void ApplyDuplicationSearchFilter(
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

                return (String.GetFileType(duplicatedNameAssetPath) == assetType &&
                        String.GetFileType(duplicatedNameAssetPath) != "")&&
                       (String.ContainsString(duplicatedNameAssetPath, "Resources") ||
                        String.ContainsString(duplicatedNameAssetPath, setting.AssetBundlesSavePath));

            }).ToList();
        }

        /// <summary>
        /// 중복 에셋이 있음을 경고 + 중복 에셋의 경로를 로그합니다.
        /// </summary>
        public static void LogDuplicationMsg(string assetName, List<string> duplicatedNameAssetGUIDs)
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
                UnityDebug.Log($"{nameof(ResourceGUIDUpdater)}.cs에서 리소스 중복을 확인.");
            }
        }
    }
}