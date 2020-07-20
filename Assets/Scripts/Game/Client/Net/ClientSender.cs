using UnityEngine;
using Zyq.Game.Base;
using Zyq.Game.Base.Protocol;
using UnityEngine.Networking;

namespace Zyq.Game.Client
{
    public class ClientSender
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
                                    Quaternion v15,
                                    Login login,
                                    LoginData data)
        {
        }

        [Send(NetMsgId.Msg_Create_Player2)]
        public static void _____RpcArray(int v1, int[] v2, string[] v3, Login[] v4, Vector3[] v5, LoginData[] v6)
        {
        }

        public static void RpcArray(int v1, int[] v2, string[] v3, LoginData[] v4)
        {
            NetworkWriter writer = new NetworkWriter();

            writer.Write(v1);

            int length1 = v2 != null ? v2.Length : 0;
            writer.Write(length1);
            if (length1 > 0)
            {
                for (int i = 0; i < length1; ++i)
                {
                    writer.Write(v2[i]);
                }
            }

            int length2 = v3 != null ? v3.Length : 0;
            writer.Write(length2);
            if (length2 > 0)
            {
                for (int i = 0; i < length2; ++i)
                {
                    writer.Write(v3[i]);
                }
            }

            int length3 = v4 != null ? v4.Length : 0;
            writer.Write(length3);
            if (length3 > 0)
            {
                for (int i = 0; i < length3; ++i)
                {
                    v4[i].testWrite(writer);
                }
            }
        }
    }
}