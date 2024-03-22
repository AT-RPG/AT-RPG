using System;
using System.IO;

namespace AT_RPG
{
    public static class String
    {
        /// <summary>
        /// 파일의 확장명을 추출합니다.
        /// </summary>
        public static string GetFileType(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        /// <summary>
        /// 파일의 확장명을 추출합니다.
        /// </summary>
        public static string GetFileName(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// 문자열에서 특정 문자열이 있는지 확인합니다.
        /// </summary>
        public static bool ContainsString(string source, string toFind)
        {
            return source.Contains(toFind);
        }

        public static bool ContainsFolder(string path, string folderName)
        {
            // 시스템에 맞는 디렉토리 구분자로 경로를 분리합니다.
            string[] pathParts = path.Split(Path.DirectorySeparatorChar);

            // 분리된 경로의 각 부분을 순회하며 폴더 이름을 확인합니다.
            foreach (string part in pathParts)
            {
                if (part.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 루트 경로와 파일 이름을 통해 파일 경로를 생성합니다. <br/>
        /// + (옵션) 파일 확장명을 변경합니다.
        /// </summary>
        public static string CreateFilePath(string rootPath, string fileName, string extension = null)
        {
            string path = Path.Combine(rootPath, fileName);
            return extension == null ? path : Path.ChangeExtension(path, extension);
        }
    }
}
