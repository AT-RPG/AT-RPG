#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Microsoft.CodeAnalysis;
using System.Text;

namespace AT_RPG
{
    /// <summary>
    /// '<see cref="UnityEngine.SceneManagement"/>' 빌드 설정이 수정되는 경우 호출하는 이벤트를 관리하는 클래스
    /// </summary>
    [InitializeOnLoad]
    public static class SceneManagerSettingsGenerator
    {
        static SceneManagerSettingsGenerator()
        {
            EditorBuildSettings.sceneListChanged += UpdateSceneManagerSettings;
        }

        /// <summary>
        /// 어드레서블에 등록된 <see cref="UnityEngine.SceneManagement.Scene"/>을 바인딩하는 변수, 씬에서 사용하는 어드레서블 라벨 리스트 변수를 자동-생성합니다.
        /// </summary>
        private static void UpdateSceneManagerSettings()
        {
            
        }
    }
}

#endif