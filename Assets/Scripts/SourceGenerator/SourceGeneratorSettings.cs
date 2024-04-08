#if UNITY_EDITOR

using System.IO;
using System.Reflection;
using System.Xml.Linq;
using UnityEditor;

namespace AT_RPG
{
    [InitializeOnLoad]
    public class SourceGeneratorSettings : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            string assemblyName = Path.GetFileNameWithoutExtension(path);
            if (assemblyName != Assembly.GetExecutingAssembly().GetName().Name) { return content; }

            XDocument doc = XDocument.Parse(content);
            XNamespace ns = "http://schemas.microsoft.com/developer/msbuild/2003";
            XElement root = doc.Element(ns + "Project");
            {
                XElement sourceGeneratorProperties = new XElement(ns + "PropertyGroup");
                sourceGeneratorProperties.Add(new XElement(ns + "EmitCompilerGeneratedFiles", "true"));
                sourceGeneratorProperties.Add(new XElement(ns + "CompilerGeneratedFilesOutputPath", $"Assets/Scripts/SourceGenerator"));

                root.Add(sourceGeneratorProperties);
            }

            return doc.ToString();
        } 
    }
}

#endif
