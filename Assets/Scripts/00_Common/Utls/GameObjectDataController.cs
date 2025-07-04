using AT_RPG;
using UnityEngine;

/// <summary>
/// 
/// 설명 : 세이브 로드 시 사용되는 필수 컴포넌트                              <br/>
/// 이 컴포넌트를 Prefab에 부착 해야, GameObject가 SaveLoad의 대상이 됨      <br/>
/// Transform정보와 자기 자신에 대한 리소스 정보를 가짐                       <br/> <br/>
/// 
/// 사용 시 주의 사항 :                                                    <br/>
/// 1. Prefab의 최상위 GameObject에만 부착                                  <br/>
/// 2. Prefab이 에셋 번들의 리소스여야 함
/// 
/// </summary>
public class GameObjectDataController : MonoBehaviour, ISaveLoadData
{
    // 게임 오브젝트의 기본 정보를 저장
    private GameObjectData gameObjectData;

    private void Awake()
    {
        gameObjectData = new GameObjectData(this);
    }



    /// <summary>
    /// DataManager가 직렬화 하기 직전에 현재 상태를 저장하고 전달
    /// </summary>
    public SerializableData SaveData()
    {
        // GameObject 기본 정보 저장
        gameObjectData.InstanceName = gameObject.name;
        gameObjectData.InstanceLayer = gameObject.layer;
        gameObjectData.InstanceTag = gameObject.tag;
        gameObjectData.InstanceIsActive = gameObject.activeInHierarchy;
        gameObjectData.InstanceHideFlags = (int)gameObject.hideFlags;
        gameObjectData.InstanceIsStatic = gameObject.isStatic;

        // Trasnform 기본 정보 저장
        gameObjectData.LocalPosition = transform.localPosition;
        gameObjectData.LocalRotation = transform.localRotation;
        gameObjectData.LocalScale = transform.localScale;

        return gameObjectData;
    }

    /// <summary>
    /// DataManager가 불러온 data로 초기화
    /// </summary>
    public void LoadData(SerializableData data)
    {
        GameObjectData gameObjectData = data as GameObjectData;
        this.gameObjectData = gameObjectData;

        // GameObject 기본 정보 복원
        gameObject.name = gameObjectData.InstanceName;
        gameObject.layer = gameObjectData.InstanceLayer;
        gameObject.tag = gameObjectData.InstanceTag;
        gameObject.SetActive(gameObjectData.InstanceIsActive);
        gameObject.hideFlags = (HideFlags)gameObjectData.InstanceHideFlags;
        gameObject.isStatic = gameObjectData.InstanceIsStatic;

        // Transform 기본 정보 복원
        transform.localPosition = gameObjectData.LocalPosition;
        transform.localRotation = gameObjectData.LocalRotation;
        transform.localScale = gameObjectData.LocalScale;
    }
}
