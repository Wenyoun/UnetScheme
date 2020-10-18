using UnityEngine;
using Zyq.Game.Base;

namespace Zyq.Game.Server
{
    public class ServerProtocolHandler : IProtocolHandler
    {
        public Connection Connection { get; set; }


        public void Register()
        {
        }

        public void Unregister()
        {
        }
    }
}