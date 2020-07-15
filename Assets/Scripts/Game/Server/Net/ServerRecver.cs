using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;

namespace Zyq.Game.Server
{
    public class ServerRecver
    {
        [Recv(NetMsgId.Msg_Login_Req)]
        public static void OnLoginHandler(Connection connection,
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
            Login login,
            LoginData data)
        {
            Debug.Log(v1 + ":" + v2 + ":" + v3 + ":" + v4 + ":" + v5 + ":" + v6 + ":" + v7 + ":" + v8 + ":" + v9 + ":" + v10 + ":" + v11 + ":" + v12 + ":" + v13 + ":" + v14 + ":" + v15 + ":" + login + ":" + data.Username + "," + data.Password);

            ServerSender.RpcTargetLoginResult(connection, true);

            Vector3 position = new Vector3(1, 0, 1);

            Entity entity = EntityFactory.CreatePlayer(connection, position);

            Server.Ins.EntityMgr.AddEntity(entity);

            ServerSender.BroadcastCreatePlayer(entity.Eid, entity.Gid, position);
        }
    }
}