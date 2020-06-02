using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ClientProtocolHandler : IProtocolHandler
    {
        public void Register(Connection connection)
        {
            connection.RegisterHandler(MsgId.Msg_Login_Res, (NetworkMessage msg) =>
            {
                LoginResProtocol res = msg.ReadMessage<LoginResProtocol>();
                Debug.Log("登陆结果:" + res.Result);
            });

            connection.RegisterHandler(MsgId.Create_Local_Player, (NetworkMessage msg) =>
            {
                Debug.Log("创建本地对象...");
            });

            connection.RegisterHandler(MsgId.Create_Remote_Player, (NetworkMessage msg) =>
            {
            });

            connection.Send(MsgId.Msg_Login_Req, new LoginRepProtocol("yinhuayong", "huayong"));
        }

        public void Unregister(Connection connection)
        {
            connection.UnregisterHandler(MsgId.Msg_Login_Res);
        }
    }
}