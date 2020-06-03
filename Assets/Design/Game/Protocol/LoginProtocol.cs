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

        public LoginResProtocol()
        {
        }

        public LoginResProtocol(short result)
        {
            Result = result;
        }

        public override void Deserialize(NetworkReader reader)
        {
            Result = reader.ReadInt16();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Result);
        }
    }

    public class CreateLocalPlayerProtocol : MessageBase
    {
        public int Eid;
        public int Gid;

        public CreateLocalPlayerProtocol()
        {
        }

        public CreateLocalPlayerProtocol(int eid, int gid)
        {
            Eid = eid;
            Gid = gid;
        }

        public override void Deserialize(NetworkReader reader)
        {
            Eid = reader.ReadInt32();
            Gid = reader.ReadInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Eid);
            writer.Write(Gid);
        }
    }

    public class CreateRemotePlayerProtocol : MessageBase
    {
        public int Eid;
        public int Gid;

        public CreateRemotePlayerProtocol()
        {
        }

        public CreateRemotePlayerProtocol(int eid, int gid)
        {
            Eid = eid;
            Gid = gid;
        }

        public override void Deserialize(NetworkReader reader)
        {
            Eid = reader.ReadInt32();
            Gid = reader.ReadInt32();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.Write(Eid);
            writer.Write(Gid);
        }
    }
}