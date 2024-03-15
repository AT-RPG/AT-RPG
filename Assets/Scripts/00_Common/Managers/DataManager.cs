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
    /// </summary>
    public partial class DataManager : Singleton<DataManager>
    {
        // 매니저 기본 설정
        [SerializeField] private static DataManagerSetting setting;

        // 세이브 파일 저장중
        private static bool isSaving = false;

        // 세이브 파일 로딩중
        private static bool isLoading = false;

        // 시작 조건 콜백
        public delegate bool StartCondition();

        // 세이브 성공 시 콜백
        public delegate void SaveCompleted();

        /// <summary>
        /// 로드 성공 시 콜백  <br/>
        /// </summary>
        /// <param name="serializedGameObjects">로드 성공 시 전달되는 게임 오브젝트 직렬화 데이터 배열</param>
        public delegate void LoadGameObjectDatasCompleted(SerializedGameObjectsList serializedGameObjects);

        /// <summary>
        /// 로드 성공 시 콜백
        /// </summary>
        /// <param name="mapSettingData">로드 성공 시 전달되는 맵 설정 데이터</param>
        public delegate void LoadMapSettingDataCompleted(MapSettingData mapSettingData);




        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }




        /// <summary>
        /// 폴더(= 맵 이름)에 현재 씬에 있는 모든 세이브 대상(GameObjectDataController.cs를 가진 게임 오브젝트) 세이브 파일을 생성합니다.
        /// </summary>
        public static void SaveAllGameObjectsCoroutine(
            string rootPath, string mapName, StartCondition started = null, SaveCompleted completed = null)
        {
            if (isSaving)
            {
                Debug.LogError("데이터를 세이브 중입니다.");
                return;
            }

            // 폴더(= 맵 이름)를 생성
            // 폴더에 게임 오브젝트 데이터 파일 정리
            string mapSaveDataPath = Path.Combine(rootPath, mapName);
            if (!Directory.Exists(mapSaveDataPath))
            {
                CreateSaveDataDirectroy(rootPath, mapName);
            }
            else
            {
                DeleteSaveDatas(rootPath, mapName, setting.gameObjectDataFileExtension);
            }

            // 게임 오브젝트 세이브 파일 생성
            Instance.StartCoroutine(
                InternalSaveAllGameObjectsCoroutine(mapSaveDataPath, started, completed));
        }

        /// <summary>
        /// SaveAllGameObjectsCoroutine 구현
        /// </summary>
        private static IEnumerator InternalSaveAllGameObjectsCoroutine(
            string mapSaveDataPath, StartCondition started = null, SaveCompleted completed = null)
        {
            // 시작 조건
            while (started != null && !started.Invoke())
            {
                yield return null;
            }
            isSaving = true;

            foreach (var gameObjectToSave in FindGameObjectsToSave())
            {
                // 게임 오브젝트 세이브 데이터 컨테이너
                List<GameObjectData> serializableDatas = new List<GameObjectData>();

                // 인터페이스로 각 스크립트가 구현한 세이브 데이터 생성
                var componentsToSave = FindScriptsToSave(gameObjectToSave);
                foreach (var component in componentsToSave)
                {
                    ISaveLoadData iSaveLoadData = component as ISaveLoadData;
                    serializableDatas.Add(iSaveLoadData.SaveData());
                }

                // 게임 오브젝트 세이브 파일 경로 지정
                string gameObjectDataFilePath = String.CreateFilePath(
                    mapSaveDataPath,
                    gameObjectToSave.GetInstanceID().ToString(),
                    setting.gameObjectDataFileExtension);

                // 세이브 파일 생성
                SerializeGameObjectDatas(serializableDatas, gameObjectDataFilePath);

                yield return null;
            }

            isSaving = false;
            yield return null;
            completed?.Invoke();
        }

        /// <summary>
        /// 현재 씬에서 게임 세이브 대상인 스크립트들을 탐색
        /// </summary>
        private static MonoBehaviour[] FindScriptsToSave(GameObject gameObject)
        {
            return gameObject.GetComponents<MonoBehaviour>()
                .Where(component => component is ISaveLoadData).ToArray();
        }

        /// <summary>
        /// 현재 씬에서 게임 세이브 대상인 게임 오브젝트들을 탐색
        /// </summary>
        private static GameObject[] FindGameObjectsToSave()
        {
            return FindObjectsOfType<GameObject>()
                .Where(go => go.GetComponents<GameObjectDataController>().Length >= 1).ToArray();
        }

        /// <summary>
        /// Json으로 게임 오브젝트 세이브 파일을 생성합니다.
        /// </summary>
        private static void SerializeGameObjectDatas(List<GameObjectData> datas, string gameObjectDataFilePath)
        {
            using (FileStream stream = new FileStream(gameObjectDataFilePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string datasToJson = JsonSerialization.ToJson(datas);
                writer.WriteLine(datasToJson);
            }
        }




        /// <summary>
        /// 폴더(= 맵 이름)에 게임 오브젝트 세이브 파일을 읽어서 데이터 컨테이너를 생성합니다.      <br/>
        /// CAUTION : ResourceManager가 씬의 리소스들을 먼저 로딩해야합니다.                     <br/>
        /// </summary>
        public static void LoadAllGameObjectsCoroutine(
            string rootPath, string dirNameToLoad, StartCondition started = null, LoadGameObjectDatasCompleted completed = null)
        {
            if (isLoading)
            {
                Debug.LogError("데이터를 로드 중입니다.");
                return;
            }

            // 게임 오브젝트 세이브 파일들 불러오기
            string mapSaveDataPath = Path.Combine(rootPath, dirNameToLoad);
            string[] mapGameObjectDataFilePaths;
            if (!Directory.Exists(mapSaveDataPath))
            {
                Debug.LogError("디렉토리 찾을 수 없음 : " + mapSaveDataPath);
                return;
            }
            else
            {
                mapGameObjectDataFilePaths 
                    = Directory.GetFiles(mapSaveDataPath, "*." + setting.gameObjectDataFileExtension);
            }

            // 세이브된 인스턴스 배열 생성
            Instance.StartCoroutine(
                InternalLoadGameObjectDatasCoroutine(mapGameObjectDataFilePaths, started, completed));
        }

        private static IEnumerator InternalLoadGameObjectDatasCoroutine(
            string[] filePaths, StartCondition started = null, LoadGameObjectDatasCompleted completed = null)
        {
            // 시작 조건
            while (started != null && !started.Invoke())
            {
                yield return null;
            }
            isLoading = true;

            // 파일 경로는 각각 GameObject 정보를 가짐
            SerializedGameObjectsList serializedGameObjects = new SerializedGameObjectsList();
            foreach (var filePath in filePaths)
            {
                // 세이브 데이터로 인스턴스 복원
                List<GameObjectData> serializableDatas = DeserializeGameObjectDatas(filePath);
                serializedGameObjects.Add(serializableDatas);

                yield return null;
            }

            isLoading = false;
            yield return null;
            completed?.Invoke(serializedGameObjects);
        }

        private static GameObjectRootData FindGameObjectRootData(List<GameObjectData> datas)
        {
            foreach (var data in datas)
            {
                GameObjectRootData gameObjectData = data as GameObjectRootData;
                if (gameObjectData != null)
                {
                    return gameObjectData;
                }
            }

            return null;
        }

        private static List<GameObjectData> DeserializeGameObjectDatas(string gameObjectDataFilePath)
        {
            string datasToJson;
            using (FileStream stream = new FileStream(gameObjectDataFilePath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                datasToJson = reader.ReadToEnd();
            }

            return JsonSerialization.FromJson<List<GameObjectData>>(datasToJson);
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
                GameObjectRootData gameObjectData =
                    FindGameObjectRootData(serializableDatas);

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
        /// 폴더(= 맵 이름)에 맵 설정 데이터 파일을 생성합니다.
        /// </summary>
        public static void SaveMapSettingDataCoroutine(
            string rootPath, MapSettingData mapSettingData, StartCondition started = null, SaveCompleted completed = null)
        {
            if (isSaving)
            {
                Debug.LogError("데이터를 세이브 중입니다.");
                return;
            }

            // 폴더(= 맵 이름)를 생성
            // 폴더에 맵 설정 세이브 파일 정리
            string mapSaveDataPath = Path.Combine(rootPath, mapSettingData.mapName);
            if (!Directory.Exists(mapSaveDataPath))
            {
                CreateSaveDataDirectroy(rootPath, mapSettingData.mapName);
            }
            else
            {
                DeleteSaveDatas(rootPath, mapSettingData.mapName, setting.mapSettingDataFileExtension);
            }

            Instance.StartCoroutine(
                InternalSaveMapSettingDataCoroutine(mapSaveDataPath, mapSettingData, started, completed));
        }

        private static IEnumerator InternalSaveMapSettingDataCoroutine(
            string mapSaveDataPath, MapSettingData mapSettingData, StartCondition started = null, SaveCompleted completed = null)
        {
            // 시작 조건
            while (started != null && !started.Invoke())
            {
                yield return null;
            }
            isSaving = true;

            // 맵 설정 파일 생성
            SerializeMapSetting(mapSaveDataPath, mapSettingData);

            isSaving = false;
            yield return null;
            completed?.Invoke();
        }

        private static void SerializeMapSetting(string mapSaveDataPath, MapSettingData mapSettingData)
        {
            string mapSettingDataFilePath = String.CreateFilePath(
                mapSaveDataPath,
                mapSettingData.mapName,
                setting.mapSettingDataFileExtension);   

            using (FileStream stream = new FileStream(mapSettingDataFilePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string datasToJson = JsonSerialization.ToJson(mapSettingData);
                writer.WriteLine(datasToJson);
            }
        }



        /// <summary>
        /// 폴더(= 맵 이름)에 맵 설정 파일을 읽어서 MapSettingData 클래스를 생성합니다.      <br/>
        /// </summary>
        public static void LoadMapSettingDataCoroutine(
            string rootPath, string mapName, StartCondition started = null, LoadMapSettingDataCompleted completed = null)
        {
            if (isLoading)
            {
                Debug.LogError("데이터를 로드 중입니다.");
                return;
            }

            // 맵 설정 세이브 파일 불러오기
            string mapSaveDataPath = Path.Combine(rootPath, mapName);
            string mapSettingDataFilePath;
            if (!Directory.Exists(mapSaveDataPath))
            {
                Debug.LogError("디렉토리 찾을 수 없음 : " + mapSaveDataPath);
                return;
            }
            else
            {
                mapSettingDataFilePath
                    = Directory.GetFiles(mapSaveDataPath, "*." + setting.mapSettingDataFileExtension)[0];
            }

            // 맵 설정 세이브 파일 생성
            Instance.StartCoroutine(
                InternalLoadMapSettingDataCoroutine(mapSettingDataFilePath, started, completed));
        }

        private static IEnumerator InternalLoadMapSettingDataCoroutine(
            string mapSettingDataFilePath, StartCondition started = null, LoadMapSettingDataCompleted completed = null)
        {
            // 시작 조건
            while (started != null && !started.Invoke())
            {
                yield return null;
            }
            isLoading = true;

            // 맵 설정 클래스 생성
            MapSettingData mapSettingData = DeserializeMapSetting(mapSettingDataFilePath);

            isLoading = false;
            yield return null;
            completed?.Invoke(mapSettingData);
        }

        private static MapSettingData DeserializeMapSetting(string mapSettingDataPath)
        {
            string dataFromJson;
            using (FileStream stream = new FileStream(mapSettingDataPath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                dataFromJson = reader.ReadToEnd();
            }

            return JsonSerialization.FromJson<MapSettingData>(dataFromJson);
        }




        /// <summary>
        /// 경로에 세이브 폴더를 만듭니다.
        /// </summary>
        public static string CreateSaveDataDirectroy(string rootPath, string mapName)
        {
            string saveDataPath = Path.Combine(rootPath, mapName);
            if (!Directory.Exists(saveDataPath))
            {
                Directory.CreateDirectory(saveDataPath);
            }

            return saveDataPath;
        }

        /// <summary>
        /// 경로에 세이브 폴더가 있는지?
        /// </summary>
        public static bool IsSaveDataDirectroyExist(string rootPath, string mapName)
        {
            string saveDataPath = Path.Combine(rootPath, mapName);
            return Directory.Exists(saveDataPath);
        }

        /// <summary>
        /// 맵 데이터 모든 게임 오브젝트 데이터를 삭제합니다.
        /// </summary>
        public static void DeleteSaveDatas(string rootPath, string mapName, string dataExtension)
        {
            string saveDataPath = Path.Combine(rootPath, mapName);
            string searchPattern = "*." + dataExtension;
            List<string> dataFiles = Directory.GetFiles(saveDataPath, searchPattern).ToList();
            dataFiles.ForEach(dataFile => File.Delete(dataFile));
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
