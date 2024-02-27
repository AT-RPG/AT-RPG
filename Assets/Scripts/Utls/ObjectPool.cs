using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    public class ObjectPool<T> where T : Object
    {
        private Stack<T> pool;
        private T baseResource;
        private readonly System.Action<T> onTakeFromPool;
        private readonly System.Action<T> onReturnToPool;
        private readonly System.Action<T> onDestroyPoolObject;

        // 풀이 처음에 가지는 Object 개수
        private int startObjectCount = 0;

        // 풀이 최대 사이즈를 넘어가면, 더이상 풀에 저장X
        private int maxPoolSize = 0;

        public ObjectPool(Object baseResource, int startObjectCount = 0, int maxPoolSize = 10000, System.Action<T> actionOnGet = null,
            System.Action<T> actionOnRelease = null, System.Action<T> actionOnDestroy = null)
        {
            // 풀 생성
            this.startObjectCount = startObjectCount;
            this.maxPoolSize = maxPoolSize;
            pool = new Stack<T>();
            {
                this.baseResource = baseResource as T;
                for (int i = 0; i < startObjectCount; i++)
                {
                    Create();
                }
            }

            // 풀 이벤트 바인딩
            onTakeFromPool = actionOnRelease;
            onReturnToPool = actionOnGet;
            onDestroyPoolObject = actionOnDestroy;
        }

        ~ObjectPool()
        {
            while (pool.Count <= 0)
            {
                T item = pool.Pop();
                onDestroyPoolObject?.Invoke(item);
            }
        }

        /// <summary>
        /// 풀에서 아이템을 가져옵니다.
        /// </summary>
        public T Get()
        {
            T item = null;

            if (pool.Count <= 0)
            {
                item = Object.Instantiate(baseResource);
                pool.Push(item);
            }
            else
            {
                item = pool.Pop();
            }

            onTakeFromPool?.Invoke(item);

            return item;
        }

        /// <summary>
        /// 풀에 아이템을 반환합니다.
        /// </summary>
        public void Return(T item)
        {
            onReturnToPool?.Invoke(item);
            pool.Push(item);
        }

        private void Create()
        {
            pool.Push(Object.Instantiate(baseResource));
        }
    }
}