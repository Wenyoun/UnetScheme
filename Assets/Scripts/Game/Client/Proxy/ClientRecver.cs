using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public static class ClientRecver
    {
        [Recv(MsgId.Msg_Login_Res)]
        public static void OnLoginRsp(byte v1,
            bool v2,
            short v3,
            int v4,
            long v5,
            ushort v6,
            uint v7,
            ulong v8,
            float v9,
            double v10,
            string v11,
            Vector2 v12,
            Vector3 v13,
            Vector4 v14,
            Quaternion v15,
            int[] v16,
            LoginData v17)
        {
            string k = "";

            for (int i = 0; i < v16.Length; ++i)
            {
                k += v16[i] + ":";
            }

            Debug.Log("Client:" + v1 + ":" + v2 + ":" + v3 + ":" + v4 + ":" + v5 + ":" + v6 + ":" + v7 + ":" + v8 +
                      ":" + v9 + ":" +
                      v10 + ":" + v11 + ":" + v12 + ":" + v13 + ":" + v14 + ":" + v15 + "," + v17.Username + "," +
                      v17.Password + "," + v17.Final + "," + k + "," + v17.Final + "," + v17.Final2);
        }
    }
    
}