using UnityEngine;

namespace AT_RPG
{
    /// <summary>
    /// ResourceReference의 사용 예시를 보여주는 클래스입니다.  <br/>
    /// + CAUTION : ResourceReference에 바인딩하는 리소스는 꼭 리소스 폴더에 있거나, 에셋 번들 리소스에 등록되어야합니다.
    /// </summary>
    public class TestMainScene_BJW : MonoBehaviour
    {
        [SerializeField] ResourceReference<GameObject> testGameObject;

        private void Start()
        {
            Instantiate(testGameObject.Resource);
        }
    }

}
