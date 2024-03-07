using System;
using UnityEngine;

namespace AT_RPG
{
    [Serializable]
    public class GameObjectData : SerializableData
    {
        // 자신의 GameObject를 래퍼런스
        [SerializeField] public ResourceReference<GameObject> Instance;

        // GameObject 기본 정보
        [SerializeField] public string      InstanceName = "";
        [SerializeField] public int         InstanceLayer = 0;
        [SerializeField] public string      InstanceTag = "";
        [SerializeField] public bool        InstanceIsActive = false;
        [SerializeField] public int         InstanceHideFlags = 0;
        [SerializeField] public bool        InstanceIsStatic = false;

        // Trasnform 기본 정보
        [SerializeField] public Vector3     LocalPosition = Vector3.zero;
        [SerializeField] public Quaternion  LocalRotation = Quaternion.identity;
        [SerializeField] public Vector3     LocalScale = Vector3.zero;



        public GameObjectData(Component owner)
            : base(owner)
        {
            Instance = new ResourceReference<GameObject>(owner.gameObject);
        }
    }
}