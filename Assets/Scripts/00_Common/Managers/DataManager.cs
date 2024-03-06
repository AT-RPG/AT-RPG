using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AT_RPG.Manager
{
    /// <summary>
    /// 현재 씬에서 GameObjectData 컴포넌트가 부착된 모든 GameObject의 데이터를 저장
    /// </summary>
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private DataManagerSetting setting;



        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
        }



        /// <summary>
        /// 폴더를 만들고 직렬화 대상인 현재 씬의 모든 GameObject들을 Json파일로 변환
        /// </summary>
        public void SaveAs(string rootPath, string dirNameToSave)
        {
            // 디렉토리 확인
            // TODO : 파일 무결성 검사 추가
            string dirPath = System.IO.Path.Combine(rootPath, dirNameToSave);
            if (!Directory.Exists(dirPath))
            {
                CreateDirectory(rootPath, dirNameToSave);
            }
            else
            {
                // 이전 데이터파일 제거
                DeleteDirectory(rootPath, dirNameToSave);
            }

            // 현재 씬의 모든 GameObject를 직렬화
            var gameObjectsToSave = FindGameObjectsToSave();
            foreach (var gameObjectToSave in gameObjectsToSave)
            {
                // 세이브할 파일 생성
                string fileID = gameObjectToSave.GetInstanceID().ToString();
                string filePath = System.IO.Path.Combine(dirPath, fileID);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    // IData.SaveData() 실행
                    var iDatas = FindIDatas(gameObjectToSave);
                    foreach (var iData in iDatas)
                    {
                        iData.SaveData();
                    }

                    // GameObject 저장
                    SerializeGameObject(gameObjectToSave, writer);
                }
            }
        }

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
