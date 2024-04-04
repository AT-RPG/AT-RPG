using AT_RPG.Manager;
using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// ResourceReference의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// </summary>
    public class TestResourceManager : MonoBehaviour
    {
        [SerializeField] AssetReferenceResource<GameObject> testAssetBundleResource;


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ResourceManager.LoadAssetAsync<GameObject>(testAssetBundleResource.AssetGUID);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                ResourceManager.LoadAssetsAsync("Test");
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                Instantiate(testAssetBundleResource);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                ResourceManager.Unload(testAssetBundleResource.AssetGUID);
            }
        }
    }
}   
