using System;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// 게임 오브젝트의 기본 정보를 저장하는 클래스
    /// </summary>
    [Serializable]
    public class GameObjectRootData : GameObjectData
    {
        // GameObject 자기 자신 프리팹 래퍼런스
        [SerializeField] public AssetReferenceResource<GameObject> Instance;

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

        [Obsolete] public GameObjectRootData() { }
    }
}