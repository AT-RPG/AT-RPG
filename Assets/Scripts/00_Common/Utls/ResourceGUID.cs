using AT_RPG;
using Newtonsoft.Json;
using System.IO;
using UnityEngine;


public class ResourceGUID : MonoBehaviour
{
    public static ResourceGUIDMap resourceGUIDMap = null;

    /// <summary>
    /// 리소스 GUID파일이 있다면 불러옵니다.
    /// </summary>
    public static ResourceGUIDMap LoadResourceGUIDMap(string resourceGUIDsFilePath)
    {
        // GUID파일 읽기
        string resourceGUIDMapFromJson;
        using (FileStream stream = new FileStream(resourceGUIDsFilePath, FileMode.Open))
        using (StreamReader reader = new StreamReader(stream))
        {
            resourceGUIDMapFromJson = reader.ReadToEnd();
        }

        return JsonConvert.DeserializeObject<ResourceGUIDMap>(resourceGUIDMapFromJson);
    }
}
