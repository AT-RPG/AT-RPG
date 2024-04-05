using System.Collections.Generic;
using System.IO;
using Unity.Serialization.Json;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor;

namespace AT_RPG
{
    [InitializeOnLoad]
    public static class AssetGuidMapGenerator
    {
        static AssetGuidMapGenerator()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        /// <summary>
        /// 플레이 모드 실행 시, <see cref="AssetGuidMap"/>를 업데이트 합니다.
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode) { CreateGuidMapFile(); }
        }



        /// <summary>
        /// 매핑 파일을 새로 생성합니다.                                             <br/>
        ///                                                                          <br/>
        /// 경로 : <see cref="ResourceManagerSettings.GetAssetGuidMapFilePath"/>     <br/>
        /// </summary>
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
            AssetGuidMap map = new();
            AddressableAssetSettingsDefaultObject.Settings.groups.ForEach(group =>
            {
                List<AddressableAssetEntry> assetEntries = new();
                group.GatherAllAssets(assetEntries, true, true, true);
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
    }
}