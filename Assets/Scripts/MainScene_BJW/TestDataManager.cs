using AT_RPG;
using AT_RPG.Manager;
using UnityEngine;

public class TestDataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            DataManager.SaveCoroutine(DataManager.Setting.DefaultSaveFolderPath, "MainScene_BJW", null);
            Debug.Log("세이브 성공");
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            DataManager.LoadCoroutine(DataManager.Setting.DefaultSaveFolderPath, "MainScene_BJW", null,
            (SerializedGameObjectsList serializedGameObjects) =>
            {
                DataManager.InstantiateGameObjects(serializedGameObjects);
            });
            Debug.Log("로드 성공");
        }
    }
}
