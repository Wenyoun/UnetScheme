using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Client
{
    public class Sender
    {
        [Send(NetMsgId.Msg_Login_Req)]
        public static void RpcLogin(byte v1,
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
                                    Quaternion v15)
        {
        }

        [Send(NetMsgId.Msg_Create_Player1)]
        public static void RpcLogin(byte v1,
                                    int v2,
                                    Test1 v3)
        {
        }
    }

    public struct Test1
    {
        public int v1;
        public Test2 v2;
        public int v3;
        public Test2 v4;
        public string v5;

        public void tesD(UnityEngine.Networking.NetworkReader reader)
        {
        }
    }

    public struct Test2
    {
        public int v1;
        public int v2;
        public string v3;
        public string v4;
        public string v5;
        public string v6;
    }
}