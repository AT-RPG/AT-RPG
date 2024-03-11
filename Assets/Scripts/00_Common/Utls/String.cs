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
    }
}
