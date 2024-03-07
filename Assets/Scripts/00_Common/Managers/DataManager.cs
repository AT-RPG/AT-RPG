using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Serialization.Json;
using System.Collections.Generic;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 설명 : 현재 씬에서 GameObjectData 컴포넌트가 부착된 모든 GameObject의 데이터를 저장 <br/> <br/>
    /// 
    /// 주의 사항 :  <br/>
    /// 1. 세이브 대상이 에셋 번들에 등록 필요  <br/>
    /// 2. 세이브에 GameObjectData 컴포넌트가 필요 <br/>
    /// 3. GameObjectData 이외에 추가적으로 데이터를 저장하려는 경우, <br/>
    /// 클래스 생성 -> 클래스에 [Serializable]와 ISaveLoadData 구현 -> 저장할려는 변수는 [SerializeField]를 사용
    /// </summary>
    public partial class DataManager : Singleton<DataManager>
    {
        // 매니저 기본 설정
        [SerializeField] private DataManagerSetting setting;

        // 세이브 파일 저장/불러오기중
        private bool isLoading = false;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }



        /// <summary>
        /// 현재 씬에 대한 디렉토리 + 세이브 파일 생성
        /// </summary>
        public void SaveAsCor(string rootPath, string dirNameToSave)
        {
            // 디렉토리 확인
            // TODO : 파일 무결성 검사 추가
            string dirPath = Path.Combine(rootPath, dirNameToSave);
            if (!Directory.Exists(dirPath))
            {
                CreateDirectory(rootPath, dirNameToSave);
            }
            else
            {
                DeleteDirectory(rootPath, dirNameToSave);
            }

            isLoading = true;

            // 세이브 파일 생성
            StartCoroutine(InternalSaveAsCor(dirPath, FindGameObjectsToSave()));
        }

        ///// <summary>
        ///// 데이터를 읽고, GameObject를 현재Scene에 인스턴싱합니다.
        ///// </summary>
        //public void Load(string rootPath, string dirNameToLoad)
        //{
        //    // 디렉토리 확인
        //    string dirPath = Path.Combine(rootPath, dirNameToLoad);
        //    if (!Directory.Exists(dirPath))
        //    {
        //        Debug.LogError("디렉토리 찾을 수 없음 : " + dirPath);
        //        return;
        //    }

        //    // Scene에 인스턴싱할 GameObject 배열
        //    GameObject[] gameObjectInstances = new GameObject[filePaths.Length];

        //    // 데이터 읽기 루프
        //    for (int i = 0; i < filePaths.Length; i++)
        //    {
        //        // 파일 열기
        //        using (FileStream stream = new FileStream(filePaths[i], FileMode.Open))
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            // Json데이터를 통해 GameObject 생성
        //            string gameObjectDataToJson = reader.ReadToEnd();
        //            SceneSerialization.FromJsonOverride(gameObjectDataToJson, ref gameObjectInstances[i]);
        //        }
        //    }

        //    // 모든 IData.LoadData() 실행
        //    foreach (var gameObjectInstance in gameObjectInstances)
        //    {
        //        var componentsToSave = FindComponentsToSave(gameObjectInstance);
        //        foreach (var component in componentsToSave)
        //        {
        //            component.LoadData();
        //        }
        //    }
        //}

        /// <summary>
        /// 현재 씬에서 GameObjectData 컴포넌트를 가진 모든 GameObject들을 Json파일로 변환
        /// </summary>
        private IEnumerator InternalSaveAsCor(string dirPath, GameObject[] gameObjectsToSave)
        {
            foreach (var gameObjectToSave in gameObjectsToSave)
            {
                // 세이브할 파일 생성
                string fileID = gameObjectToSave.GetInstanceID().ToString();
                string filePath = Path.Combine(dirPath, fileID);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {

                    // 인터페이스로 각 스크립트가 구현한 세이브 데이터 생성
                    var componentsToSave = FindComponentsToSave(gameObjectToSave);
                    List<SerializableData> serializableDatas = new List<SerializableData>();
                    foreach (var component in componentsToSave)
                    {
                        ISaveLoadData iSaveLoadData = component as ISaveLoadData;
                        serializableDatas.Add(iSaveLoadData.SaveData());
                    }

                    SerializeGameObjectData(serializableDatas, writer);
                }

                yield return null;
            }

            isLoading = false;
        }

        /// <summary>
        /// GameObject를 Json으로 저장합니다.
        /// </summary>
        private void SerializeGameObjectData(List<SerializableData> datas, StreamWriter writer)
        {
            string datasToJson = JsonSerialization.ToJson(datas);
            writer.WriteLine(datasToJson);
        }

        /// <summary>
        /// 게임 세이브 대상인 스크립트들을 탐색
        /// </summary>
        private MonoBehaviour[] FindComponentsToSave(GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>()
                .Where(component => component is ISaveLoadData).ToArray();
        }

        /// <summary>
        /// 게임 세이브 대상인 게임 오브젝트들을 탐색
        /// </summary>
        private GameObject[] FindGameObjectsToSave()
        {
            return FindObjectsOfType<GameObject>()
                .Where(go => go.GetComponents<GameObjectDataController>().Length >= 1).ToArray();
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

    public partial class DataManager
    {
        // 세이브 파일 저장/불러오기중
        public bool IsLoading => isLoading;

        // 매니저 기본 설정
        public DataManagerSetting Setting => setting;
    }
}
