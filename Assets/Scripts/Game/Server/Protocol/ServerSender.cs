using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Proto;

namespace Zyq.Game.Server
{
    public class ServerSender
    {
        [Send(MsgId.Msg_Login_Res)]
        public static void RpcTargetLoginResult(Connection connection,
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
            Quaternion v15,
            int[] v16,
            LoginData v17)
        {
        }
    }
}