using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace AT_RPG
{
    public static class SceneManagerSettingsGenerator
    {
        static SceneManagerSettingsGenerator()
        {
            EditorBuildSettings.sceneListChanged += OnSceneManagerSettingsUpdate;
        }

        [MenuItem("AT_RPG/SceneManager/Update SceneManager Settings")]
        public static void OnSceneManagerSettingsUpdate()
        {
            string filename = $"{nameof(SceneManagerSettings)}Gen";
            string path = Path.GetDirectoryName(SceneManagerSettings.GetFilePath());
            string filePath = Path.Combine(path, filename + ".cs");

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                var sourceBuilder = new StringBuilder();
                sourceBuilder.AppendLine("using System.Collections.Generic;");
                sourceBuilder.AppendLine("using UnityEngine;");
                sourceBuilder.AppendLine("using UnityEngine.AddressableAssets;");
                sourceBuilder.AppendLine();
                sourceBuilder.AppendLine("namespace AT_RPG");
                sourceBuilder.AppendLine("{");
                sourceBuilder.AppendLine($"    public partial class {nameof(SceneManagerSettings)}");
                sourceBuilder.AppendLine("    {");
                sourceBuilder.AppendLine($"        [Space(20)]");

                foreach (var scene in EditorBuildSettings.scenes)
                {
                    string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                    sourceBuilder.AppendLine($"        [Space(10)]");
                    sourceBuilder.AppendLine($"        public readonly string {sceneName} = \"{sceneName}\";");
                    sourceBuilder.AppendLine($"        public List<{nameof(AssetLabelReference)}> {sceneName}AddressableLabelMap = new List<{nameof(AssetLabelReference)}>();");
                    sourceBuilder.AppendLine();
                }

                sourceBuilder.AppendLine("    }");
                sourceBuilder.AppendLine("}");

                writer.WriteLine(sourceBuilder.ToString());
            }
        }
    }

}