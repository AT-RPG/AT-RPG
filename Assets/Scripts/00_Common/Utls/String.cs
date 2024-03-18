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
