using System.IO;

namespace AT_RPG
{
    public static class String
    {
        /// <summary>
        /// 주어진 경로의 가장 하위 폴더의 이름을 반환
        /// </summary>
        public static string GetFolderOrFileName(string dirPath)
        {
            return Path.GetFileNameWithoutExtension(dirPath);
        }
    }
}
