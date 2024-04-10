using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AT_RPG
{
    [CreateAssetMenu(fileName = "SceneManagerSettings", menuName = "ScriptableObject/SceneManager Setting")]
    public partial class SceneManagerSettings : ScriptableObject
    {
        // 리소스 페이크 로딩 지속 시간
        public float FakeLoadingDuration;

        /// <summary>
        /// 클래스의 파일 경로를 반환합니다.
        /// </summary>
        public static string GetFilePath() => GetFilePathImpl();
        private static string GetFilePathImpl([CallerFilePath] string filePath = "") => filePath;
    }
}
