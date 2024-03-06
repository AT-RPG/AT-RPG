using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 현재 씬에서 GameObjectData 컴포넌트가 부착된 모든 GameObject의 데이터를 저장
    /// 조건 1. 리소스 매니저에 등록되어있을 것 
    /// 조건 2. GameObjectData 컴포넌트가 등록되어있을것.
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private DataManagerSetting setting;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }



        ///// <summary>
        ///// 폴더를 만들고 직렬화 대상인 현재 씬의 모든 GameObject들을 Json파일로 변환
        ///// </summary>
        //public void SaveAs(string rootPath, string dirNameToSave)
        //{
        //    // 디렉토리 확인
        //    // TODO : 파일 무결성 검사 추가
        //    string dirPath = System.IO.Path.Combine(rootPath, dirNameToSave);
        //    if (!Directory.Exists(dirPath))
        //    {
        //        CreateDirectory(rootPath, dirNameToSave);
        //    }
        //    else
        //    {
        //        // 이전 데이터파일 제거
        //        DeleteDirectory(rootPath, dirNameToSave);
        //    }

        //    // 현재 씬의 모든 GameObject를 직렬화
        //    var gameObjectsToSave = FindGameObjectsToSave();
        //    foreach (var gameObjectToSave in gameObjectsToSave)
        //    {
        //        // 세이브할 파일 생성
        //        string fileID = gameObjectToSave.GetInstanceID().ToString();
        //        string filePath = System.IO.Path.Combine(dirPath, fileID);
        //        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        //        using (StreamWriter writer = new StreamWriter(stream))
        //        {
        //            // IData.SaveData() 실행
        //            var iDatas = FindIDatas(gameObjectToSave);
        //            foreach (var iData in iDatas)
        //            {
        //                iData.SaveData();
        //            }

        //            // GameObject 저장
        //            SerializeGameObject(gameObjectToSave, writer);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 데이터를 읽고, GameObject를 현재Scene에 인스턴싱합니다.
        ///// </summary>
        //public void Load(string rootPath, string dirNameToLoad)
        //{
        //    // 디렉토리 확인
        //    string dirPath = System.IO.Path.Combine(rootPath, dirNameToLoad);
        //    if (!Directory.Exists(dirPath))
        //    {
        //        Debug.LogError("Directory not found in : " + dirPath);
        //        return;
        //    }

        //    // 데이터 확인
        //    string[] filePaths = Directory.GetFiles(dirPath);
        //    if (filePaths.Length <= 0)
        //    {
        //        Debug.LogError("Save File not found in : " + dirPath);
        //        return;
        //    }

        //    // Scene에 인스턴싱 GameObject 배열
        //    GameObject[] gameObjectInstances = new GameObject[filePaths.Length];

        //    // 데이터 읽기 루프
        //    for (int i = 0; i < filePaths.Length; i++)
        //    {
        //        // 파일 열기
        //        using (FileStream stream = new FileStream(filePaths[i], FileMode.Open))
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            // Json데이터를 통해 GameObject 생성
        //            string gameObjectToJson = reader.ReadToEnd();
        //            SceneSerialization.FromJsonOverride(gameObjectToJson, ref gameObjectInstances[i]);
        //        }
        //    }

        //    // 모든 IData.LoadData() 실행
        //    foreach (var gameObjectInstance in gameObjectInstances)
        //    {
        //        var iDatas = FindIDatas(gameObjectInstance);
        //        foreach (var iData in iDatas)
        //        {
        //            iData.LoadData();
        //        }
        //    }
        //}



        /// <summary>
        /// GameObject를 Json으로 저장합니다.
        /// </summary>
        private void SerializeGameObject(GameObject gameObject, StreamWriter writer)
        {
            string gameObjectToJson = JsonUtility.ToJson(gameObject);
            writer.WriteLine(gameObjectToJson);
        }

        /// <summary>
        /// GameObject의 모든 ISaveLoadData를 탐색
        /// </summary>
        private ISaveLoadData[] FindIDatas(GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>().OfType<ISaveLoadData>().ToArray();
        }

        /// <summary>
        /// 게임 세이브 대상인 GameObject들을 탐색
        /// </summary>
        private GameObject[] FindGameObjectsToSave()
        {
            return FindObjectsOfType<GameObject>().Where(go => go.GetComponents<GameObjectData>().Length >= 1).ToArray();
        }

        /// <summary>
        /// 경로에 세이브 폴더를 생성
        /// </summary>
        private void CreateDirectory(string rootPath, string dirNameToCreate)
        {
            string filePath = System.IO.Path.Combine(rootPath, dirNameToCreate);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }

        /// <summary>
        /// 경로에 있는 세이브 폴더 안에 모든 파일을 삭제
        /// </summary>
        private void DeleteDirectory(string rootPath, string dirNameToDelete)
        {
            string[] files = Directory.GetFiles(System.IO.Path.Combine(rootPath, dirNameToDelete));
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }
    }
}
