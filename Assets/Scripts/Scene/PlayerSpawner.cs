using UnityEngine;

namespace AT_RPG
{
    public class PlayerSpawner : MonoBehaviour
    {
        /// <summary>
        /// 플레이어가 스폰할 위치
        /// </summary>
        public Transform Point
        {
            get => point;
            set => point = value;
        }
        [SerializeField] private Transform point;

        /// <summary>
        /// 플레이어 프리팹
        /// </summary>
        public AssetReferenceResource<GameObject> PlayerPrefab
        {
            get => playerPrefab;
            set => playerPrefab = value;
        }
        [SerializeField] private AssetReferenceResource<GameObject> playerPrefab;

        /// <summary>
        /// 플레이어를 현재 씬에 인스턴싱합니다.
        /// </summary>
        public void Spawn()
        {
            Instantiate(playerPrefab, point.position, Quaternion.identity, null);
        }
    }

}