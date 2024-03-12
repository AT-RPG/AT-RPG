using AT_RPG;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace AT_RPG
{
    public static class ResourceGUID
    {
        public static ResourceGUIDMap ResourceGUIDMapCache = null;

        /// <summary>
        /// 리소스 GUID파일이 있다면 불러옵니다. <br/>
        /// + 이 클래스에 로드한 정보를 캐싱합니다.
        /// </summary>
        public static ResourceGUIDMap LoadResourceGUIDMap()
        {
            var setting = Resources.Load<ResourceManagerSetting>("ResourceManagerSettings");
            var filePath = Path.Combine(setting.ResourceGUIDMapSavePath, setting.ResourceGUIDMapFileName);

            // GUID파일 읽기
            string resourceGUIDMapFromJson;
            using (FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate))
            using (StreamReader reader = new StreamReader(stream))
            {
                resourceGUIDMapFromJson = reader.ReadToEnd();
            }

            ResourceGUIDMapCache = JsonConvert.DeserializeObject<ResourceGUIDMap>(resourceGUIDMapFromJson);

            return ResourceGUIDMapCache;
        }
    }
}