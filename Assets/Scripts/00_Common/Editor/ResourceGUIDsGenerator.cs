using System.Diagnostics;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering;
using System.Collections.Generic;

using UnityDebug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;
using System.Linq;
using Unity.Serialization.Json;
using System.IO;

namespace AT_RPG
{
    /// <summary>
    /// Key1 = 리소스 타입,  Key2 = 리소스 이름,  Key3 = 리소스에 1:1 매핑되는 GUID
    /// </summary>
    [Serializable]
    public class ResourceGUIDMap : SerializedDictionary<string, SerializedDictionary<string, Guid>> 
    {
        public bool ContainsResourceType(string resourceTypeName)
        {
            return ContainsKey(resourceTypeName);
        }

        public bool ContainsResourceName(string resourceTypeName, string resourceName)
        {
            return this[resourceTypeName].ContainsKey(resourceName);
        }
    }

    /// <summary>
    /// 리소스 폴더와 에셋 번들 리소스 폴더의 모든 리소스들에 대한 1:1 GUID를 생성하는 클래스
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
                // 매핑할 모든 리소스 로드
                List<UnityObject> resources = new List<UnityObject>();
                List<UnityObject> resourcesFromResourcesFolder = LoadAllResourcesFromResourcesFolder();
                List<UnityObject> resourcesFromAssetBundles = LoadAllResourcesFromAssetBundles();

                // 리소스 매핑
                resources.AddRange(resourcesFromResourcesFolder);
                resources.AddRange(resourcesFromAssetBundles);

                // 리소스 GUID 매핑파일 생성
                ResourceGUIDMap resourceGUIDMap = CreateResourceGUIMap(resources);
                CreateJsonFileFromResourceGUIDMap(resourceGUIDMap);

                // 모든 리소스 메모리에서 해제
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

            // 에셋 번들에 있는 모든 리소스를 resources 리스트 하나에 모읍니다.
            foreach (var assetBundleName in AssetDatabase.GetAllAssetBundleNames())
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundleName);
                resources.AddRange(assetBundle.LoadAllAssets());
            }

            return resources;
        }

        /// <summary>
        /// 리소스들에 대한 1:1 GUID 매핑 컨테이너를 생성합니다.
        /// </summary>
        /// <param name="resources"></param>
        private static ResourceGUIDMap CreateResourceGUIMap(List<UnityObject> resources)
        {
            ResourceGUIDMap resourceGUIMap = new ResourceGUIDMap();

            foreach (var resource in resources)
            {
                string resourceTypeName = resource.GetType().Name;
                string resourceName = resource.name;

                try
                {
                    if (!resourceGUIMap.ContainsResourceType(resourceTypeName))
                    {
                        resourceGUIMap[resourceTypeName] = new SerializedDictionary<string, Guid>();
                    }

                    resourceGUIMap[resourceTypeName][resourceName] = Guid.NewGuid();
                }
                catch
                {
                    Resources.UnloadUnusedAssets();
                }
            }

            return resourceGUIMap;
        }

        /// <summary>
        /// 리소스 GUID 매핑에 대한 Json 파일을 생성합니다.
        /// </summary>
        private static void CreateJsonFileFromResourceGUIDMap(ResourceGUIDMap resourceGUIDMap)
        {
            // 리소스 설정 저장 경로 불러오기
            ResourceManagerSetting setting
                = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");

            // Json으로 직렬화
            string resourceGUIDMapToJson = JsonSerialization.ToJson(resourceGUIDMap);

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