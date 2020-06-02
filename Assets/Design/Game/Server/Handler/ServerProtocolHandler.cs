using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ServerProtocolHandler : IProtocolHandler
    {
        public void Register(Connection connection)
        {
            connection.RegisterHandler(MsgId.Msg_Login_Req, (NetworkMessage msg) =>
            {
                LoginRepProtocol req = msg.ReadMessage<LoginRepProtocol>();
                if (req.Username == "yinhuayong" && req.Password == "huayong")
                {
                    connection.Send(MsgId.Msg_Login_Res, new LoginResProtocol(ProtocolResult.Success));
                    Debug.Log("登录成功");
                    connection.Send(MsgId.Create_Local_Player, new CreateLocalPlayerProtocol());
                }
                else
                {
                    connection.Send(MsgId.Msg_Login_Res, new LoginResProtocol(ProtocolResult.Error));
                }
            });
        }

        public void Unregister(Connection connection)
        {
            connection.UnregisterHandler(MsgId.Msg_Login_Req);
        }
    }
}