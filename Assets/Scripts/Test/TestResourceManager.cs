using UnityEngine;

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
            // 리소스 매니저에 바인딩된 어드레서블 리소스가 프리-캐시되어있다면, 리소스를 반환합니다.
            Instantiate(testAssetBundleResource.Resource);
        }
    }
}   
