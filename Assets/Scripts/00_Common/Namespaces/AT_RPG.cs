using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace AT_RPG
{
    #region ResourceManager

    // Key1 = 씬 이름,  Value1 = 씬에서 사용될 리소스 매핑s
    public class SceneResourceMap : Dictionary<string, ResourceMap> { }

    // Key1 = 리소스의 타입,  Key2 = 리소스의 이름,  Value1 = 리소스
    public class ResourceMap : Dictionary<string, Dictionary<string, UnityObject>> { }

    // Key1 = 리소스 이름, Key2 = 리소스 타입, Value1 = GUID
    public class ResourceGUIDMap : SerializableDictionary<string, KeyValuePair<string, SerializableGuid>> { }

    // Key1 = 씬 이름,  Value1 = 씬에 적용되는 리소스 번들s
    public class AssetBundleMap : Dictionary<string, List<AssetBundle>> { }

    #endregion

    #region InputManager

    // Key1 = 키,  Key2 = 동작 이름(ex. 앞으로, 뒤로, 발사),  Value1 = 키에 바인딩되는 델리게이트
    public class KeyMap : Dictionary<KeyCode, KeyValuePair<string, Action>> { }

    #endregion

}