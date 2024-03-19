using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System;

namespace AT_RPG
{
    [Serializable]
    public class MultiplayAuthentication 
    {
        private AuthenticationValues authenticationValues;
        public string NickName = "Guest";

        public static MultiplayAuthentication CreateNew()
        {
            MultiplayAuthentication authentication = new MultiplayAuthentication();
            authentication.authenticationValues = new AuthenticationValues(Guid.NewGuid().ToString());

            return authentication;
        }
    }
}