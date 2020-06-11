using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client {
    public static class SendServer {
        [Send(MsgId.Msg_Login_Req)]
        public static void LoginReq(Connection connection,
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
            Quaternion v15) { }
    }
}