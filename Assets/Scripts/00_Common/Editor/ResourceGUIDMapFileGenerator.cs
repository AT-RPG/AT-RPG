using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using UnityDebug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;
using System.Linq;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

namespace AT_RPG
{
    /// <summary>
    /// 리소스 폴더와 에셋 번들 리소스 폴더의 모든 리소스들에 대한 1:1 GUID를 생성하는 클래스    <br/>
    /// + 타입과 리소스의 이름을 통해 GUID를 매핑합니다.                                      <br/>
    /// </summary>
    public static class ResourceGUIDMapFileGenerator 
    {
        /// <summary>
        /// 리소스 GUID 매핑 파일을 생성합니다.
        /// </summary>
        [MenuItem("Generators/Build Resource GUIDs")]

        public static void BuildResourceGUIDMapFile()
        {
            Stopwatch stopwatch = new Stopwatch();

            // 리소스 GUID 경로 불러오기
            ResourceManagerSetting setting =
                Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            stopwatch.Start();
            {
                // 리소스 매핑 컨테이너 생성
                ResourceGUIDMap resourceGUIDMap = new ResourceGUIDMap();

                // 매핑할 리소스 폴더의 모든 리소스를 로드 + 매핑
                List<UnityObject> resourcesFromResourcesFolder = LoadAllResourcesFromResourcesFolder();
                MapResourcesToGUIDMap(resourcesFromResourcesFolder, ref resourceGUIDMap);
                Resources.UnloadUnusedAssets();

                // 매핑할 에셋 번들의 모든 리소스를 로드 + 매핑
                List<UnityObject> resourcesFromAssetBundles = LoadAllResourcesFromAssetBundles();
                MapResourcesToGUIDMap(resourcesFromAssetBundles, ref resourceGUIDMap);
                AssetBundle.UnloadAllAssetBundles(true);

                // 리소스 GUID 매핑파일 생성
                SerializeResourceGUIDMap(resourceGUIDMap);
            }
            stopwatch.Stop();

            // 함수 종료 로그
            UnityDebug.Log($"{nameof(ResourceGUIDMapFileGenerator)}.cs에서 리소스 GUID 매핑 파일 업데이트. \n" +
                           $"리소스 GUID 매핑 파일 위치 : " +
                           $"{Path.Combine(setting.ResourceGUIDMapSavePath, setting.ResourceGUIDMapFileName)} \n" +
                           $"소요시간 : " +
                           $"{stopwatch.ElapsedMilliseconds}ms \n");
        }

        /// <summary>
        /// 리소스 폴더에 있는 모든 리소스를 로드
        /// </summary>
        private static List<UnityObject> LoadAllResourcesFromResourcesFolder()
        {
            return Resources.LoadAll("").ToList();
        }

        /// <summary>
        /// 에셋 번들에 등록되어있는 모든 리소스를 로드
        /// </summary>
        private static List<UnityObject> LoadAllResourcesFromAssetBundles()
        {
            List<UnityObject> resources = new List<UnityObject>();
            ResourceManagerSetting setting 
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // 에셋 번들에 있는 모든 리소스를 resources 리스트 하나에 모읍니다.
            foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
            {
                AssetBundle assetBundle 
                    = AssetBundle.LoadFromFile(Path.Combine(setting.AssetBundlesSavePath, assetBundleName));
                resources.AddRange(assetBundle.LoadAllAssets());
                assetBundle.Unload(false);
            }

            return resources;
        }

        /// <summary>
        /// 리소스들에 대한 1:1 GUID 매핑 컨테이너를 생성합니다.
        /// </summary>
        /// <param name="resources">매핑할 리소스 리스트</param>
        /// <param name="resourceGUIDMap">매핑을 적용할 컨테이너</param>
        private static void MapResourcesToGUIDMap(
            List<UnityObject> resources, ref ResourceGUIDMap resourceGUIDMap)
        {
            // 이전에 만든 GUID파일을 가져옴
            ResourceManagerSetting setting 
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");
            ResourceGUIDMap loadedResourceGUIDMap
                = ResourceGUID.LoadResourceGUIDMap(Path.Combine(setting.ResourceGUIDMapSavePath, setting.ResourceGUIDMapFileName));

            // 에셋 타입 + 리소스이름에 GUID 매핑 시작
            foreach (var resource in resources)
            {
                string resourceTypeName = resource.GetType().Name;
                string resourceName = resource.name;

                // 처음등록되는 에셋 타입임?
                if (!resourceGUIDMap.ContainsResourceType(resourceTypeName))
                {
                    resourceGUIDMap[resourceTypeName] = new Dictionary<string, Guid>();
                }

                // 이전 GUID에 등록된적 있음?
                // true  -> 이전 GUID 그대로
                // false -> 새로 GUID 생성
                if (loadedResourceGUIDMap != null &&
                    loadedResourceGUIDMap.ContainsResourceType(resourceTypeName) &&
                    loadedResourceGUIDMap.ContainsResourceName(resourceTypeName, resourceName))
                {
                    resourceGUIDMap[resourceTypeName][resourceName] 
                        = loadedResourceGUIDMap[resourceTypeName][resourceName];
                }
                else
                {
                    resourceGUIDMap[resourceTypeName][resourceName] = Guid.NewGuid();
                }
            }
        }

        /// <summary>
        /// 리소스 GUID 매핑에 대한 Json 파일을 생성합니다.
        /// </summary>
        private static void SerializeResourceGUIDMap(ResourceGUIDMap resourceGUIDMap)
        {
            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // Json으로 직렬화
            string resourceGUIDMapToJson = JsonConvert.SerializeObject(resourceGUIDMap, Formatting.Indented);
            

            // 파일 경로 설정
            string filePath 
                = Path.Combine(setting.ResourceGUIDMapSavePath, setting.ResourceGUIDMapFileName);

            // 파일에 Json 쓰기
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(resourceGUIDMapToJson);
            }
        }
    }
}