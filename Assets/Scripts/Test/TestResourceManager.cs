using AT_RPG.Manager;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AT_RPG
{
    /// <summary>
    /// ResourceReference의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// </summary>
    public class TestResourceManager : MonoBehaviour
    {
        [SerializeField] AssetReferenceResource<GameObject> testAssetBundleResource;

        private void Start()
        {

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                var handle = ResourceManager.LoadAssetsAsync("Test");
                handle.Completed += handle => { if (handle.Status == AsyncOperationStatus.Succeeded) { Instantiate(handle.Result[0]); } };
            }
        }
    }
}   
