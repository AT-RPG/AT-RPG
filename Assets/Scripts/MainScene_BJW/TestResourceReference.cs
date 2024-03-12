using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// ResourceReference의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// + CAUTION 1 : ResourceReference에 바인딩하는 리소스는 꼭 리소스 폴더에 있거나, 에셋 번들 리소스에 등록되어야합니다.   <br/>
    /// + CAUTION 2 : 꼭 리소스 폴더에 있거나, 에셋 번들 리소스에 변경이 있다면 에디터 메뉴의 Generator에서 빌드를 해주세요.   <br/>
    /// </summary>
    public class TestResourceReference : MonoBehaviour
    {
        /// <summary>
        /// 에셋 번들의 리소스
        /// </summary>
        [SerializeField] ResourceReference<GameObject> testAssetBundleResource;

        /// <summary>
        /// 리소스 폴더의 리소스
        /// </summary>
        [SerializeField] ResourceReference<GameObject> testResourcesFolderResource;


        private void Start()
        {
            Instantiate(testAssetBundleResource.Resource);
            Instantiate(testResourcesFolderResource.Resource);
        }
    }
}   
