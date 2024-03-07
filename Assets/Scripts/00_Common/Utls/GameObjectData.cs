using System;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 
    /// 설명 : 세이브 로드 시 사용되는 필수 컴포넌트                              <br/>
    /// 이 컴포넌트를 Prefab에 부착 해야, GameObject가 SaveLoad의 대상이 됨      <br/>
    /// Transform정보와 자기 자신에 대한 리소스 정보를 가짐                       <br/> <br/>
    /// 
    /// 사용 시 주의 사항 :                                                    <br/>
    /// 1. Prefab의 최상위 GameObject에만 부착                                  <br/>
    /// 2. Prefab이 에셋 번들의 리소스일 것
    /// 
    /// </summary>
    [Serializable]
    public class GameObjectData : MonoBehaviour, ISaveLoadData
    {
        // 자신의 GameObject를 래퍼런스
        [HideInInspector]
        [SerializeField] private ResourceReference<GameObject> instance;

        // GameObject 기본 정보
        [HideInInspector]
        [SerializeField] private string instanceName = "";
        [HideInInspector]
        [SerializeField] private int instanceLayer = 0;
        [HideInInspector]
        [SerializeField] private string instanceTag = "";
        [HideInInspector]
        [SerializeField] private bool instanceIsActive = false;
        [HideInInspector]
        [SerializeField] private int instanceHideFlags = 0;
        [HideInInspector]
        [SerializeField] private bool instanceIsStatic = false;

        // Trasnform 기본 정보
        [HideInInspector]
        [SerializeField] private Vector3 localPosition = Vector3.zero;
        [HideInInspector]
        [SerializeField] private Quaternion localRotation = Quaternion.identity;
        [HideInInspector]
        [SerializeField] private Vector3 localScale = Vector3.zero;


        private void Awake()
        {
            instance = new ResourceReference<GameObject>(this.gameObject);
        }

        /// <summary>
        /// DataManager가 직렬화 하기 직전에 현재 상태를 저장합니다.
        /// </summary>
        public void SaveData()
        {
            // GameObject 기본 정보 저장
            instanceName = gameObject.name;
            instanceLayer = gameObject.layer;
            instanceTag = gameObject.tag;
            instanceIsActive = gameObject.activeInHierarchy;
            instanceHideFlags = (int)gameObject.hideFlags;
            instanceIsStatic = gameObject.isStatic;

            // Trasnform 기본 정보 저장
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;
            localScale = transform.localScale;
        }

        public void LoadData()
        {

        }
    }
}