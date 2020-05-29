using UnityEngine.Networking;

namespace Zyq.Game.Protocol
{
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

        public string Username = string.Empty;
        public string Password = string.Empty;

        public LoginResProtocol()
        {
        }

        public LoginResProtocol(string username, string password)
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
}