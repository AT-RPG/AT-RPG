using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

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

        /// <summary>
        /// 주어진 문자열을 클래스명이나 변수명으로 적합하게 변환합니다.
        /// </summary>
        public static string MakeValidIdentifier(string originalName)
        {
            if (string.IsNullOrEmpty(originalName))
            {
                throw new ArgumentException("Original name cannot be null or empty.");
            }

            StringBuilder builder = new StringBuilder();
            bool isFirstChar = true;

            foreach (char c in originalName)
            {
                if (isFirstChar)
                {
                    // 첫 글자는 알파벳이어야 함
                    if (char.IsLetter(c) || c == '_')
                    {
                        builder.Append(c);
                        isFirstChar = false;
                    }
                }
                else
                {
                    // 나머지 글자는 알파벳, 숫자 또는 언더스코어만 허용
                    if (char.IsLetterOrDigit(c) || c == '_')
                    {
                        builder.Append(c);
                    }
                }
            }

            // 첫 글자가 비어 있으면 기본 이름 설정
            if (builder.Length == 0)
            {
                builder.Append("Identifier");
            }

            // 숫자로 시작하는 경우 알파벳 'A'를 앞에 붙임
            if (char.IsDigit(builder[0]))
            {
                builder.Insert(0, 'A');
            }

            // 클래스명과 변수명에서 허용되지 않는 공백 및 특수문자 제거
            string identifier = Regex.Replace(builder.ToString(), @"[^a-zA-Z0-9_]", "");

            return identifier;
        }
    }
}
