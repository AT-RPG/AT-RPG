using System;
using System.IO;
using AT_RPG.Manager;
using Unity.Serialization.Json;

namespace AT_RPG
{
    [Serializable]
    public class MultiplayAuthentication 
    {
        public string               GUID;
        public string               NickName = "Guest";

        /// <summary>
        /// 클라이언트 인증 식별자를 새로 초기화 합니다. 
        /// </summary>
        public static MultiplayAuthentication CreateNew(string nickName = "Guest")
        {
            MultiplayAuthentication authentication = new MultiplayAuthentication();
            authentication.GUID = System.Guid.NewGuid().ToString();
            authentication.NickName = nickName;

            authentication.Save();

            return authentication;
        }

        /// <summary>
        /// 기존의 클라이언트 인증 식별자가 있는지 확인합니다. <br/>
        /// 인증 정보 위치 : <see cref="MultiplayManagerSetting.AuthenticationDataPath"/>
        /// </summary>
        public static bool IsExist()
        {
            string filePath = Path.Combine(MultiplayManager.Setting.AuthenticationDataPath, "Authentication");
            return File.Exists(filePath);
        }

        /// <summary>
        /// 클라이언트 인증 식별자 클래스를 저장합니다. <br/>
        /// 인증 정보 위치 : <see cref="MultiplayManagerSetting.AuthenticationDataPath"/>
        /// </summary>
        public void Save()
        {
            string filePath = Path.Combine(MultiplayManager.Setting.AuthenticationDataPath, "Authentication");
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                string dataToJson = JsonSerialization.ToJson(this);
                writer.WriteLine(dataToJson);
            }
        }

        /// <summary>
        /// 기존의 클라이언트 인증 식별자 클래스를 불러옵니다. <br/>
        /// 인증 정보 위치 : <see cref="MultiplayManagerSetting.AuthenticationDataPath"/>
        /// </summary>
        public static MultiplayAuthentication Load()
        {
            string dataFromJson;
            string filePath = Path.Combine(MultiplayManager.Setting.AuthenticationDataPath, "Authentication");
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                dataFromJson = reader.ReadToEnd();
            }

            return JsonSerialization.FromJson<MultiplayAuthentication>(dataFromJson);
        }
    }
}