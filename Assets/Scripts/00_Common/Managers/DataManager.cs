using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace AT_RPG.Manager
{
    public class DataManager : Singleton<DataManager>
    {
        [SerializeField] private DataManagerSetting setting;

        protected override void Awake()
        {
            base.Awake();

            setting = Resources.Load<DataManagerSetting>("DataManagerSettings");
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
