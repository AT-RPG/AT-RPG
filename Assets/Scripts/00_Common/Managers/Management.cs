using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AT_RPG.Manager
{
    public class SceneResourceMap : Dictionary<string, ResourceMap> { }
    public class ResourceMap : Dictionary<string, Object> { }
    public class AssetBundleMap : Dictionary<string, List<AssetBundle>> { }
}
