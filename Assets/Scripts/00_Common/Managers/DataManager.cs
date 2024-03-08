using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Serialization.Json;
using System.Collections.Generic;
using DG.Tweening;

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

        // 세이브 파일 저장중
        private bool isSaving = false;

        // 세이브 파일 로딩중
        private bool isLoading = false;

        // 세이브 성공 시 콜백
        public delegate void OnSaveCompleted();

        /// <summary>
        /// 로드 성공 시 콜백
        /// </summary>
        /// <param name="loadedGameObject">로드 성공 시 반환되는 인스턴스 배열</param>
        public delegate void OnLoadCompleted(List<GameObject> loadedGameObject);



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }



        /// <summary>
        /// 현재 씬에 대한 디렉토리 + 각 세이브 대상 인스턴스 마다 세이브 파일 생성
        /// </summary>
        public void SaveAsCor(
            string rootPath, string dirNameToSave, OnSaveCompleted completedCallback)
        {
            // 디렉토리 사용 가능하게 정리
            string dirPath = Path.Combine(rootPath, dirNameToSave);
            if (!Directory.Exists(dirPath))
            {
                CreateDirectory(rootPath, dirNameToSave);
            }
            else
            {
                DeleteDirectory(rootPath, dirNameToSave);
            }

            isSaving = true;

            // 세이브 파일 생성
            StartCoroutine(InternalSaveAsCor(dirPath, FindGameObjectsToSave(), completedCallback));
        }


        /// <summary>
        /// 세이브 파일을 읽어서 인스턴스를 생성
        /// CAUTION : ResourceManager가 씬의 리소스들을 먼저 로딩해야합니다.
        /// </summary>
        public void LoadCor(
            string rootPath, string dirNameToLoad, OnLoadCompleted completedCallback)
        {
            // 인스턴스 세이브 파일들 불러오기
            string dirPath = Path.Combine(rootPath, dirNameToLoad);
            string[] filePaths;
            if (!Directory.Exists(dirPath))
            {
                Debug.LogError("디렉토리 찾을 수 없음 : " + dirPath);
                return;
            }
            else
            {
                filePaths = Directory.GetFiles(dirPath);
            }

            isLoading = true;

            // 세이브된 인스턴스 배열 생성
            StartCoroutine(InternalLoadCor(filePaths, completedCallback));
        }



        /// <summary>
        /// 현재 씬에서 GameObjectData 컴포넌트를 가진 모든 GameObject들을 Json파일로 변환
        /// </summary>
        private IEnumerator InternalSaveAsCor(
            string dirPath, GameObject[] gameObjectsToSave, OnSaveCompleted completedCallback)
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

                    SerializeDatas(serializableDatas, writer);
                }

                yield return null;
            }

            completedCallback?.Invoke();

            isSaving = false;
        }


        /// <summary>
        /// 세이브된 파일로 인스턴스를 생성
        /// </summary>
        private IEnumerator InternalLoadCor(
            string[] filePaths, OnLoadCompleted completedCallback)
        {
            List<GameObject> loadedGameObjects = new List<GameObject>();

            // 파일 경로는 각각 GameObject 정보를 가짐
            foreach (var filePath in filePaths)
            {
                // 세이브된 파일 읽기
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 세이브 데이터로 인스턴스 복원
                    var serializableDatas = DeserializeDatas(reader);
                    GameObjectData gameObjectData = FindGameObjectDataAtSerializableDatas(serializableDatas);
                    GameObject loadedGameObject = LoadGameObjectFromDatas(gameObjectData, serializableDatas);

                    loadedGameObjects.Add(loadedGameObject);
                }

                yield return null;
            }

            completedCallback?.Invoke(loadedGameObjects);

            isLoading = false;
        }

        /// <summary>
        /// 세이브 데이터로 인스턴스 복원
        /// </summary>
        private GameObject LoadGameObjectFromDatas(GameObjectData gameObjectData, List<SerializableData> datas)
        {
            // ResourceReference<GameObject> Instance로 인스턴싱
            // CAUTION : 이부분에서 CurrentScene이 문제가 생길 수 있음
            // TODO : 인스턴싱 문제
            GameObject loadedGameObject = Instantiate(gameObjectData.Instance.Resource);

            foreach (var data in datas)
            {
                ILoadData saveLoad 
                    = loadedGameObject.GetComponent(data.ComponentTypeName) as ILoadData;

                saveLoad.LoadData(data);
            }

            return loadedGameObject;
        }

        /// <summary>
        /// Json파일에서 SerializableData를 불러옵니다.
        /// </summary>
        private List<SerializableData> DeserializeDatas(StreamReader reader)
        {
            string datasToJson = reader.ReadToEnd();
            return JsonSerialization.FromJson<List<SerializableData>>(datasToJson);
        }   


        /// <summary>
        /// GameObject를 Json으로 저장합니다.
        /// </summary>
        private void SerializeDatas(List<SerializableData> datas, StreamWriter writer)
        {
            string datasToJson = JsonSerialization.ToJson(datas);
            writer.WriteLine(datasToJson);
        }


        /// <summary>
        /// 필수 GameObjectData를 가장 먼저 읽기 위해 사용됩니다.
        /// </summary>
        private GameObjectData FindGameObjectDataAtSerializableDatas(
            List<SerializableData> datas)
        {
            foreach (var data in datas)
            {
                GameObjectData gameObjectData = data as GameObjectData;
                if (gameObjectData != null)
                {
                    return gameObjectData;
                }
            }

            return null;
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
        // 세이브 파일 저장중
        public bool IsSaving => isSaving;

        // 세이브 파일 로딩중
        public bool IsLoading => isLoading;

        // 매니저 기본 설정
        public DataManagerSetting Setting => setting;
    }
}
