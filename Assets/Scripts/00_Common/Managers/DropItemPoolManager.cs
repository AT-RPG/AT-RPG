using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 필드에 Drop되는 아이템을 뿌려주는 pool매니저
    /// </summary>
    public class DropItemPoolManager : MonoBehaviour
    {
        public static DropItemPoolManager Instance;

        [Header("Pool되어질 프리팹")]
        public Item[] items;

        private System.Random random = new();
        private List<GameObject>[] pools;

        void Awake() 
        {
            Instance = this;

            pools = new List<GameObject>[items.Length];

            for(int i = 0; i < pools.Length; i++)
            {
                pools[i] = new List<GameObject>();
            }
        }

        /// <summary>
        /// 오브젝트의 타입에 따라 다른 아이템을 드랍시켜줌
        /// </summary>
        /// <param name="_dropType">DropType.Monster나 DropType.Destructible중 하나를 전달</param>
        /// <param name="tr">전달해주는 오브젝트의 transform을 전달</param>
        public void Get(DropType _dropType, Transform tr)
        {
            switch (_dropType)
            {
                case DropType.Monster:
                for(int i = 0; i < (int)DropType.Monster; i++)
                {
                    if(RandomItemSetActive(items[i].dropRate))
                    {
                        Pool(i, tr);
                    }
                }
                break;
                case DropType.Destructible:
                for(int i = 3; i < (int)DropType.Destructible + i; i++)
                {
                    if(RandomItemSetActive(items[i].dropRate))
                    {
                        Pool(i, tr);
                    }
                }
                break;
            }
        }

        /// <summary>
        /// pool안에 오브젝트가 있으면 Active True, 없으면 Instantiate 시켜줌
        /// </summary>
        private GameObject Pool(int _index, Transform tr)
        {
            GameObject obj = null;

            foreach (GameObject item in pools[_index])
            {
                if(!item.activeSelf)
                {
                    obj = item;
                    Vector3 randomPosition = Random.insideUnitSphere * 2f;
                    randomPosition.y = 0f;
                    obj.transform.position = tr.position + randomPosition;
                    obj.SetActive(true);
                    break;
                }
            }

            if(obj == null)
            {
                Vector3 randomPosition = Random.insideUnitSphere * 2f;
                randomPosition.y = 0f;
                obj = Instantiate(items[_index].itemPrefab, tr.position + randomPosition, Quaternion.identity, transform);
                pools[_index].Add(obj);
            }
            return obj;
        }

        /// <summary>
        /// DropItem의 드랍률로 계산해 true와 false를 반환
        /// </summary>
        private bool RandomItemSetActive(int _dropRate)
        {
            int percent = _dropRate;
            int randomValue = random.Next(101);

            if(randomValue < percent) return true;
            
            return false;
        }
    }
}
