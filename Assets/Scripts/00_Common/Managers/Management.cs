using System;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace AT_RPG.Manager
{
    // Key1 = 씬 이름, Value1 = 씬에서 사용될 리소스 매핑s
    public class SceneResourceMap : Dictionary<string, ResourceMap> { }

    // Key1 = 리소스의 타입, Key2 = 리소스의 이름, Value1 = 리소스
    public class ResourceMap : Dictionary<string, Dictionary<string, UnityObject>> { }

    // Key1 = 씬 이름, Value1 = 씬에 적용되는 리소스 번들s
    public class AssetBundleMap : Dictionary<string, List<AssetBundle>> { }
}
