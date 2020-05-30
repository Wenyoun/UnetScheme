using UnityEngine.Networking;

namespace Zyq.Game.Protocol
{
    public class ProtocolResult
    {
        public const short Success = 1;
        public const short Error = 2;
    }

    public class LoginRepProtocol : MessageBase
    {
        public string Username = string.Empty;
        public string Password = string.Empty;

        public LoginRepProtocol()
        {
        }

        public LoginRepProtocol(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public override void Deserialize(NetworkReader reader)
        {
            Username = reader.ReadString();
            Password = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Username);
            writer.Write(Password);
        }
    }

    public class LoginResProtocol : MessageBase
    {
        public short Result = -1;
        public string Username = string.Empty;
        public string Password = string.Empty;

        public LoginResProtocol()
        {
        }

        public LoginResProtocol(short result, string username, string password)
        {
            Result = result;
            Username = username;
            Password = password;
        }

        public override void Deserialize(NetworkReader reader)
        {
            Result = reader.ReadInt16();
            Username = reader.ReadString();
            Password = reader.ReadString();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Result);
            writer.Write(Username);
            writer.Write(Password);
        }
    }
}