using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server {
    public class RecvClient {
        [Recv(MsgId.Msg_Login_Req)]
        public static void OnLogin(Connection connection,
            byte v1,
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
            Quaternion v15) {
            Debug.Log(v1);
            Debug.Log(v2);
            Debug.Log(v3);
            Debug.Log(v4);
            Debug.Log(v5);
            Debug.Log(v6);
            Debug.Log(v7);
            Debug.Log(v8);
            Debug.Log(v9);
            Debug.Log(v10);
            Debug.Log(v11);
            Debug.Log(v12);
            Debug.Log(v13);
            Debug.Log(v14);
            Debug.Log(v15);
        }
    }
}