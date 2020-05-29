using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Server
{
    public class ServerProtocolRegister : IProtocolRegister
    {
        public void Register(NetworkConnection net)
        {
            net.RegisterHandler(MsgId.Msg_Login_Req, (NetworkMessage msg) =>
            {
                LoginRepProtocol req = msg.ReadMessage<LoginRepProtocol>();
                Debug.Log("登陆请求: Username=" + req.Username + ",Password=" + req.Password);
                net.Send(MsgId.Msg_Login_Res, new LoginResProtocol(req.Username + "用户名", req.Password + "密码"));
            });
        }

        public void Unregister(NetworkConnection net)
        {
            net.UnregisterHandler(MsgId.Msg_Login_Req);
        }
    }
}