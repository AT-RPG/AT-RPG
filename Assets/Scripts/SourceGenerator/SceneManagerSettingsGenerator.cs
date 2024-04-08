#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Compilation;


namespace AT_RPG
{
    /// <summary>
    /// '<see cref="UnityEngine.SceneManagement"/>' 씬 빌드 설정이 수정되는 경우 호출하는 이벤트를 관리하는 클래스
    /// </summary>
    [InitializeOnLoad]
    public static class SceneManagerSettingsGenerator
    {
        static SceneManagerSettingsGenerator()
        {
            EditorBuildSettings.sceneListChanged += () =>
            {
                CompilationPipeline.RequestScriptCompilation(RequestScriptCompilationOptions.CleanBuildCache);
            };
        }
    }
}

#endif