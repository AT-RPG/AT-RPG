using System.Diagnostics;
using System.IO;
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
            // 리소스 GUID 저장 경로 가져오기
            ResourceManagerSetting setting =
                Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            Stopwatch stopwatch = new Stopwatch();
            bool isDirty = false;
            stopwatch.Start();
            {
                // 리소스 폴더나 에셋 번들 리소스에서 변경사항이 나타남?
                // TRUE : 리소스 GUID 매핑 파일 재빌드
                isDirty = CheckIsResourceDirty(importedAssets, deletedAssets, movedAssets);
                if (isDirty)
                {
                    ResourceGUIDMapFileGenerator.BuildResourceGUIDMapFile();
                    ResourceGUID.LoadResourceGUIDMap
                        (Path.Combine(setting.ResourceGUIDMapSavePath, setting.ResourceGUIDMapFileName));
                }
            }
            stopwatch.Stop();

            if (isDirty)
            {
                UnityDebug.Log($"{nameof(ResourceGUIDUpdater)}.cs에서 리소스 GUID 매핑 업데이트. \n" +
                               $"리소스 GUID 매핑 캐시 위치 : " +
                               $"{nameof(ResourceGUID)}.{nameof(ResourceGUID.ResourceGUIDMapCache)} \n" +
                               $"소요시간 : " +
                               $"{stopwatch.ElapsedMilliseconds}ms \n");
            }

            Resources.UnloadUnusedAssets();
        }

        /// <summary>
        /// 리소스 폴더나 에셋 번들 리소스에 업데이트가 일어났는지 확인합니다.
        /// </summary>
        private static bool CheckIsResourceDirty(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets)
        {
            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // 변경사항이 Imported인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = importedAssets.Where(asset =>
                {
                    return String.ContainsString(asset, "Resources") ||
                           String.ContainsString(asset, setting.AssetBundlesSavePath);
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            // 변경사항이 deleted인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = deletedAssets.Where(asset =>
                {
                    return String.ContainsString(asset, "Resources") ||
                           String.ContainsString(asset, setting.AssetBundlesSavePath);
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            // 변경사항이 moved인 에셋에 리소스나 에셋번들 리소스 변경이 있음?
            {
                var filteredAssets = movedAssets.Where(asset =>
                {
                    return String.ContainsString(asset, "Resources") ||
                           String.ContainsString(asset, setting.AssetBundlesSavePath);
                }).ToList();
                if (filteredAssets.Count >= 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}