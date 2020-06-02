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
            connection.Net.RegisterHandler(MsgId.Msg_Login_Req, (NetworkMessage msg) =>
            {
                LoginRepProtocol req = msg.ReadMessage<LoginRepProtocol>();
                Debug.Log("登陆请求: Username=" + req.Username + ",Password=" + req.Password);
                if (req.Username == "yinhuayong" && req.Password == "huayong")
                {
                    connection.Net.Send(MsgId.Msg_Login_Res, new LoginResProtocol(ProtocolResult.Success, req.Username + "用户名", req.Password + "密码"));
                }
                else
                {
                    connection.Net.Send(MsgId.Msg_Login_Res, new LoginResProtocol(ProtocolResult.Error, string.Empty, string.Empty));
                }
            });
        }

        public void Unregister(Connection connection)
        {
            connection.Net.UnregisterHandler(MsgId.Msg_Login_Req);
        }
    }
}