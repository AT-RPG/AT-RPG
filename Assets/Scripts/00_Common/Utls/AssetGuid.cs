using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Serialization.Json;

#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;
#endif

namespace AT_RPG
{
    public partial class AssetGuidMap
    {
        // 어드레서블 에셋 매핑 캐시
        private static Dictionary<string, string> map = new();

#if UNITY_EDITOR
        [MenuItem("AT_RPG/Addressables/Generate AssetGuid Map")]
        public static void CreateGuidMapFile()
        {
            // 파일 저장 경로를 가져오기 위해 설정을 로드
            ResourceManagerSettings setting = Resources.Load<ResourceManagerSettings>($"{nameof(ResourceManagerSettings)}");
            if (!setting)
            {
                Debug.Log($"{nameof(ResourceManagerSettings)}이 {nameof(Resources)}에 없습니다.");
            }

            // 모든 어드레서블의 에셋을 매핑
            map = new();
            AddressableAssetSettingsDefaultObject.Settings.groups.ForEach(group =>
            {
                List<AddressableAssetEntry> assetEntries = new();
                group.GatherAllAssets(assetEntries, false, true, true);
                assetEntries.ForEach(entry => map.Add(entry.AssetPath, entry.guid));
            });

            // 데이터를 Json파일 직렬화
            using (FileStream stream = new FileStream(setting.GetAssetGuidMapFilePath(), FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string mapToJson = JsonSerialization.ToJson(map);
                writer.WriteLine(mapToJson);
            }
            Debug.Log($"{nameof(AssetGuidMap)}파일 생성 완료. 경로 : {setting.GetAssetGuidMapFilePath()}");

            // 파일 저장 경로를 가져오기 위해 사용했던 설정을 언로드
            Resources.UnloadAsset(setting);
        }
#endif

        public static void LoadGuidMapFile()
        {
            // 파일 저장 경로를 가져오기 위해 설정을 로드
            ResourceManagerSettings setting = Resources.Load<ResourceManagerSettings>($"{nameof(ResourceManagerSettings)}");
            if (!setting)
            {
                Debug.Log($"{nameof(ResourceManagerSettings)}이 {nameof(Resources)}에 없습니다.");
            }

            // Json파일의 Guid 매핑을 역직렬화
            using (FileStream stream = new FileStream(setting.GetAssetGuidMapFilePath(), FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                string mapFromJson = reader.ReadToEnd();
                map = JsonSerialization.FromJson<Dictionary<string, string>>(mapFromJson);
            }
            Debug.Log($"{nameof(AssetGuidMap)} 불러오기 완료.");

            // 파일 저장 경로를 가져오기 위해 사용했던 설정을 언로드
            Resources.UnloadAsset(setting);
        }
    }

    public partial class AssetGuidMap
    {
        public static Dictionary<string, string> Map => map;
    }
}