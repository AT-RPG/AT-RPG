using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 설명 : 세이브 로드 시 사용되는 필수 컴포넌트  <br/>
    /// 이 컴포넌트를 Prefab에 부착 시, SaveLoad의 대상이 됨 <br/>
    /// Transform정보와 자기 자신에 대한 리소스 정보를 가짐 <br/> <br/>
    /// 
    /// 사용 시 주의 사항 : <br/>
    /// 1. Prefab의 최상위 GameObject에만 부착
    /// 2. Prefab이 에셋 번들의 리소스일 것
    /// </summary>
    public class GameObjectData : MonoBehaviour, ISaveLoadData
    {
        // 자기 자신의 GameObject를 래퍼런스
        // 이 변수를 통해서 SaveLoad시 인스턴싱
        private ResourceReference<GameObject> instance;

        // Trasnform 정보
        private Vector3 localPosition = Vector3.zero;

        private void Awake()
        {
            instance = new ResourceReference<GameObject>(this.gameObject);
        }

        public void SaveData()
        {
            
        }

        public void LoadData()
        {

        }
    }
}