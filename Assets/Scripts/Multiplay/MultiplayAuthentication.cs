using UnityEngine;
using System;
using System.IO;
using AT_RPG.Manager;
using Newtonsoft.Json;

namespace AT_RPG
{
    [Serializable]
    public partial class MultiplayAuthentication 
    {
        [SerializeField] private string               guid;
        [SerializeField] private string               nickName = "Guest";

        /// <summary>
        /// 클라이언트 인증 식별자를 새로 초기화 합니다. 
        /// </summary>
        public static MultiplayAuthentication CreateNew(string nickName = "Guest")
        {
            MultiplayAuthentication authentication = new MultiplayAuthentication();
            authentication.guid = Guid.NewGuid().ToString();
            authentication.nickName = nickName;

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
                string dataToJson = JsonConvert.SerializeObject(this, Formatting.Indented);
                writer.WriteLine(dataToJson);
            }
        }

        /// <summary>
        /// 기존의 클라이언트 인증 식별자 클래스를 불러옵니다. <br/>
        /// 인증 정보 위치 : <see cref="MultiplayManagerSetting.AuthenticationDataPath"/>
        /// </summary>
        public static MultiplayAuthentication Load()
        {
            MultiplayAuthentication authentication;

            string filePath = Path.Combine(MultiplayManager.Setting.AuthenticationDataPath, "Authentication");
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (StreamReader reader = new StreamReader(stream))
            {
                string dataFromJson = reader.ReadToEnd();
                authentication =  JsonConvert.DeserializeObject<MultiplayAuthentication>(dataFromJson);
            }

            authentication.Save();

            return authentication;
        }
    }

    public partial class MultiplayAuthentication
    {
        public string GUID => guid;

        public string NickName
        {
            get => nickName;
            set => nickName = value;
        }
    }

}