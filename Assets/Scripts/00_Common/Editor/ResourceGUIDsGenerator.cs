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
    /// 리소스 폴더와 에셋 번들 리소스 폴더의 모든 리소스들에 대한 1:1 GUID를 생성하는 클래스 <br/>
    /// </summary>
    public static class ResourceGUIDsGenerator 
    {
        /// <summary>
        /// 리소스 GUID 매핑 파일을 생성합니다.
        /// </summary>
        [MenuItem("Generators/Build Resource GUIDs")]

        public static void BuildResourceGUIDs()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            {
                List<UnityObject> resources = new List<UnityObject>();
                List<UnityObject> resourcesFromResourcesFolder = new List<UnityObject>();
                List<UnityObject> resourcesFromAssetBundles = new List<UnityObject>();
                try
                {
                    // 매핑할 모든 리소스 로드
                    // 로드된 리소스들을 하나로 통합
                    resourcesFromResourcesFolder = LoadAllResourcesFromResourcesFolder();
                    resourcesFromAssetBundles = LoadAllResourcesFromAssetBundles();
                    resources.AddRange(resourcesFromResourcesFolder);
                    resources.AddRange(resourcesFromAssetBundles);

                    // 리소스 매핑 컨테이너 생성
                    // 리소스 GUID 매핑파일 생성
                    ResourceGUIDMap resourceGUIDMap = CreateResourceGUIMap(resources);
                    SerializeResourceGUIDMap(resourceGUIDMap);
                }
                catch
                {
                    UnityDebug.Log($"리소스 로딩 실패");
                }

                // 리소스 메모리 해제
                Resources.UnloadUnusedAssets();
            }
            stopwatch.Stop();

            UnityDebug.Log($"{nameof(ResourceGUIDsGenerator)}.cs에서 리소스 GUID 생성완료." +
                      $" 소요시간 : {stopwatch.ElapsedMilliseconds}ms");
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
        /// <param name="resources"></param>
        private static ResourceGUIDMap CreateResourceGUIMap(List<UnityObject> resources)
        {
            // 이전에 만든 GUID파일을 가져옴
            ResourceManagerSetting setting 
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");
            ResourceGUIDMap loadedResourceGUIDMap
                = ResourceGUID.LoadResourceGUIDMap(Path.Combine(setting.ResourceGUIDsSavePath, setting.ResourceGUIDsFileName));

            
            // 새로 작성할 GUID 컨테이너 선언
            ResourceGUIDMap resourceGUIMap = new ResourceGUIDMap();

            // 에셋 타입 + 리소스이름에 GUID 매핑 시작
            foreach (var resource in resources)
            {
                string resourceTypeName = resource.GetType().Name;
                string resourceName = resource.name;

                // 처음등록되는 에셋 타입임?
                if (!resourceGUIMap.ContainsResourceType(resourceTypeName))
                {
                    resourceGUIMap[resourceTypeName] = new Dictionary<string, Guid>();
                }

                // 이전 GUID에 등록된적 있음?
                // true  -> 이전 GUID 그대로
                // false -> 새로 GUID 생성
                if (loadedResourceGUIDMap != null &&
                    loadedResourceGUIDMap[resourceTypeName].ContainsKey(resourceName))
                {
                    resourceGUIMap[resourceTypeName][resourceName] 
                        = loadedResourceGUIDMap[resourceTypeName][resourceName];
                }
                else
                {
                    resourceGUIMap[resourceTypeName][resourceName] = Guid.NewGuid();
                }
            }

            return resourceGUIMap;
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
                = Path.Combine(setting.ResourceGUIDsSavePath, setting.ResourceGUIDsFileName);

            // 파일에 Json 쓰기
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                writer.WriteLine(resourceGUIDMapToJson);
            }
        }
    }

}