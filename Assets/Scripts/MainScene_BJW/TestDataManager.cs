using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// DataManager의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// + CAUTION : 저장 대상은 꼭 리소스 폴더에 있거나, 에셋 번들 리소스에 등록되어야합니다.
    /// + CAUTION : 저장 대상은 꼭 'GameObjectDataController.cs' 컴포넌트를 소유해야 합니다.
    /// </summary>
    public class TestDataManager : MonoBehaviour
    {
        void Update()
        {
            // 현재 씬의 저장 대상을 유니티의 Application.persistentData 경로에 "DirToSave" 폴더를 만들어서 직렬화합니다.
            if (Input.GetKeyDown(KeyCode.F5))
            {
                DataManager.SaveCoroutine(DataManager.Setting.DefaultSaveFolderPath, "DirToSave");
                Debug.Log("세이브 성공");
            }

            // "DirToSave" 폴더에 있는 직렬화 데이터를 가져와서 인스턴싱합니다.
            if (Input.GetKeyDown(KeyCode.F6))
            {
                DataManager.LoadCoroutine(DataManager.Setting.DefaultSaveFolderPath, "DirToSave", null,
                (SerializedGameObjectsList serializedGameObjects) =>
                {
                    DataManager.InstantiateGameObjects(serializedGameObjects);
                });
                Debug.Log("로드 성공");
            }
        }
    }
}
