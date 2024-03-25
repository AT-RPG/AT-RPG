using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// SaveLoadManager의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// + CAUTION : 저장 대상은 꼭 리소스 폴더에 있거나, 에셋 번들 리소스에 등록되어야합니다.
    /// + CAUTION : 저장 대상은 꼭 'GameObjectDataController.cs' 컴포넌트를 소유해야 합니다.
    /// </summary>
    public class TestSaveLoadManager : MonoBehaviour
    {
        void Update()
        {
            // 현재 씬의 저장 대상을 유니티의 Application.persistentData 경로에 "DirToSave" 폴더를 만들어서 직렬화합니다.
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveLoadManager.SaveWorldSettingData(
                    SaveLoadManager.Setting.defaultSaveFolderPath, SaveLoadManager.WorldSettingData, () => !SaveLoadManager.IsSaving);

                SaveLoadManager.SaveGameObjectDatas(
                    SaveLoadManager.Setting.defaultSaveFolderPath, SaveLoadManager.WorldSettingData.worldName, () => !SaveLoadManager.IsSaving);

                Debug.Log("세이브 성공");
            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                SaveLoadManager.LoadWorldSettingDataCoroutine(
                    SaveLoadManager.Setting.defaultSaveFolderPath, SaveLoadManager.WorldSettingData.worldName, 
                    () => !SaveLoadManager.IsLoading,
                    loadedWorldSettingData => SaveLoadManager.WorldSettingData = loadedWorldSettingData);

                SaveLoadManager.LoadGameObjectDatasCoroutine(
                   SaveLoadManager.Setting.defaultSaveFolderPath, SaveLoadManager.WorldSettingData.worldName,
                   () => !SaveLoadManager.IsLoading,
                   loadedGameObjectDatas => SaveLoadManager.InstantiateGameObjectFromData(loadedGameObjectDatas));


                Debug.Log("로드 성공");
            }
        }
    }
}
