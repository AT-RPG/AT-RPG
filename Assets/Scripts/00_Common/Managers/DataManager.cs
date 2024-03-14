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
    /// 1. 세이브 대상이 에셋 번들이나 리소스 폴더에 등록 필요  <br/>
    /// 2. 세이브 대상에 GameObjectData 컴포넌트가 필요 <br/>
    /// 3. GameObjectData 이외에 추가적으로 데이터를 저장하려는 경우, <br/>
    /// 클래스 생성 -> 클래스에 [Serializable]와 ISaveLoadData 구현 -> 저장할려는 변수는 [SerializeField]를 사용
    /// </summary>
    public partial class DataManager : Singleton<DataManager>
    {
        // 매니저 기본 설정
        [SerializeField] private static DataManagerSetting setting;

        // 세이브 파일 저장중
        private static bool isSaving = false;

        // 세이브 파일 로딩중
        private static bool isLoading = false;

        // 세이브 성공 시 콜백
        public delegate void SaveCompletionCallback();

        /// <summary>
        /// 로드 성공 시 콜백
        /// </summary>
        /// <param name="serializedGameObjects">로드 성공 시 반환되는 게임오브젝트 직렬화 데이터 배열</param>
        public delegate void LoadCompletionCallback(SerializedGameObjectsList serializedGameObjects);

        // 시작 조건 콜백 true->시작, false->대기
        public delegate bool StartConditionCallback();



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }



        /// <summary>
        /// 현재 씬에 대한 디렉토리 + 각 세이브 대상 인스턴스 마다 세이브 파일 생성
        /// </summary>
        public static void SaveCoroutine(
            string rootPath, string dirNameToSave, StartConditionCallback started = null, SaveCompletionCallback completed = null)
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

            // 세이브 파일 생성
            GameObject[] gameObjectsToSave = FindGameObjectsToSave();
            Instance.StartCoroutine(InternalSaveCoroutine(dirPath, gameObjectsToSave, started, completed));
        }

        /// <summary>
        /// 세이브 파일을 읽어서 인스턴스를 생성                                <br/>
        /// CAUTION : ResourceManager가 씬의 리소스들을 먼저 로딩해야합니다.    <br/>
        /// </summary>
        public static void LoadCoroutine(
            string rootPath, string dirNameToLoad, StartConditionCallback started = null, LoadCompletionCallback completed = null)
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

            // 세이브된 인스턴스 배열 생성
            Instance.StartCoroutine(InternalLoadCoroutine(filePaths, started, completed));
        }

        /// <summary>
        /// 세이브 데이터로 인스턴스 복원
        /// </summary>
        public static void InstantiateGameObjects(
            SerializedGameObjectsList serializedGameObjectsList)
        {
            foreach (var serializableDatas in serializedGameObjectsList)
            {
                // 에셋번들에서 인스턴스 원본을 찾기 위해 GameObjectData를 먼저 찾기
                GameObjectData gameObjectData =
                    FindGameObjectDataAtSerializableDatas(serializableDatas);

                // ILoadData 인터페이스로 인스턴스를 복원
                GameObject instanceFromSaveData = Instantiate(gameObjectData.Instance.Resource);
                foreach (var data in serializableDatas)
                {
                    ILoadData saveLoad
                        = instanceFromSaveData.GetComponent(data.ComponentTypeName) as ILoadData;

                    saveLoad.LoadData(data);
                }
            }
        }



        /// <summary>
        /// 현재 씬에서 GameObjectData 컴포넌트를 가진 모든 GameObject들을 Json파일로 변환
        /// </summary>
        private static IEnumerator InternalSaveCoroutine(
            string dirPath, GameObject[] gameObjectsToSave, StartConditionCallback started = null, SaveCompletionCallback completed = null)
        {
            isSaving = true;

            // 시작 조건
            if (started != null)
            {
                while (!started.Invoke())
                {
                    yield return null;
                }
            }

            foreach (var gameObjectToSave in gameObjectsToSave)
            {
                // 세이브할 파일 생성
                string fileID = gameObjectToSave.GetInstanceID().ToString();
                string filePath = Path.Combine(dirPath, fileID);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // 직렬화 할 세이브 데이터 컨테이너
                    List<SerializableData> serializableDatas = new List<SerializableData>();

                    // 인터페이스로 각 스크립트가 구현한 세이브 데이터 생성
                    var componentsToSave = FindComponentsToSave(gameObjectToSave);
                    foreach (var component in componentsToSave)
                    {
                        ISaveLoadData iSaveLoadData = component as ISaveLoadData;
                        serializableDatas.Add(iSaveLoadData.SaveData());
                    }

                    SerializeDatas(serializableDatas, writer);
                }

                yield return null;
            }

            isSaving = false;
            completed?.Invoke();
        }

        /// <summary>
        /// 세이브된 파일로 인스턴스를 생성
        /// </summary>
        private static IEnumerator InternalLoadCoroutine(
            string[] filePaths, StartConditionCallback started = null, LoadCompletionCallback completed = null)
        {
            isLoading = true;

            // 시작 조건
            if (started != null)
            {
                while (!started.Invoke())
                {
                    yield return null;
                }
            }

            // 파일 경로는 각각 GameObject 정보를 가짐
            SerializedGameObjectsList serializedGameObjects = new SerializedGameObjectsList();
            foreach (var filePath in filePaths)
            {
                // 세이브된 파일 읽기
                using (FileStream stream = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // 세이브 데이터로 인스턴스 복원
                    List<SerializableData> serializableDatas = DeserializeDatas(reader);
                    serializedGameObjects.Add(serializableDatas);
                }

                yield return null;
            }


            isLoading = false;
            completed?.Invoke(serializedGameObjects);
        }

        /// <summary>
        /// 필수 GameObjectData를 가장 먼저 읽기 위해 사용됩니다.
        /// </summary>
        private static GameObjectData FindGameObjectDataAtSerializableDatas(
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
        /// Json파일에서 SerializableData를 불러옵니다.
        /// </summary>
        private static List<SerializableData> DeserializeDatas(StreamReader reader)
        {
            string datasToJson = reader.ReadToEnd();
            return JsonSerialization.FromJson<List<SerializableData>>(datasToJson);
        }   

        /// <summary>
        /// GameObject를 Json으로 저장합니다.
        /// </summary>
        private static void SerializeDatas(List<SerializableData> datas, StreamWriter writer)
        {
            string datasToJson = JsonSerialization.ToJson(datas);
            writer.WriteLine(datasToJson);
        }

        /// <summary>
        /// 게임 세이브 대상인 스크립트들을 탐색
        /// </summary>
        private static MonoBehaviour[] FindComponentsToSave(GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>()
                .Where(component => component is ISaveLoadData).ToArray();
        }

        /// <summary>
        /// 게임 세이브 대상인 게임 오브젝트들을 탐색
        /// </summary>
        private static GameObject[] FindGameObjectsToSave()
        {
            return FindObjectsOfType<GameObject>()
                .Where(go => go.GetComponents<GameObjectDataController>().Length >= 1).ToArray();
        }

        /// <summary>
        /// 경로에 세이브 폴더를 생성
        /// </summary>
        private static void CreateDirectory(string rootPath, string dirNameToCreate)
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
        private static void DeleteDirectory(string rootPath, string dirNameToDelete)
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
        public static bool IsSaving => isSaving;

        // 세이브 파일 로딩중
        public static bool IsLoading => isLoading;

        // 매니저 기본 설정
        public static DataManagerSetting Setting => setting;
    }
}
