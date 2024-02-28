using UnityEngine;

namespace AT_RPG
{
    public partial class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public partial class Singleton<T>
    {
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject singletonInstance = new GameObject();
                    singletonInstance.name = typeof(T).Name;
                    singletonInstance.AddComponent<T>();
                }

                return instance;
            }
        }
    }
}