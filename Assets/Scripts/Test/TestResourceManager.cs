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
                ResourceManager.LoadAssetAsync<GameObject>(testAssetBundleResource);
                Debug.Log("f1");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                Instantiate(testAssetBundleResource);
                Debug.Log("f2");

            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                ResourceManager.Unload(testAssetBundleResource);
                Debug.Log("f3");

            }




            if (Input.GetKeyDown(KeyCode.F5))
            {
                ResourceManager.LoadAssetsAsync("Test");
                Debug.Log("f5");

            }

            if (Input.GetKeyDown(KeyCode.F6))
            {
                Instantiate(testAssetBundleResource);
                Debug.Log("f6");

            }

            if (Input.GetKeyDown(KeyCode.F7))
            {
                ResourceManager.Unload("Test");
                Debug.Log("f7");
            }
        }
    }
}   
