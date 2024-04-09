using System.IO;
using System.Text;
using UnityEditor;

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
                    sourceBuilder.AppendLine($"        public {nameof(AssetReferenceScene)} {sceneName}Asset;");
                    sourceBuilder.AppendLine($"        public List<string> {sceneName}AddressableLabelMap = new List<string>();");
                    sourceBuilder.AppendLine();
                }

                sourceBuilder.AppendLine("    }");
                sourceBuilder.AppendLine("}");

                writer.WriteLine(sourceBuilder.ToString());
            }
        }
    }

}