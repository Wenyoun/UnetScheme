using UnityEngine.Networking;

using Zyq.Game.Base;
using System.Collections.Generic;

namespace Zyq.Game.Server
{
    public class Server
    {
        public static Server Ins = new Server();

        private Dictionary<int, IProtocolRegister> m_Registers;

        public void Init()
        {
            m_Registers = new Dictionary<int, IProtocolRegister>();
            ServerObjectRegisterHandler.Register();
        }

        public void Dispose()
        {
            m_Registers.Clear();
            m_Registers = null;
            ServerObjectRegisterHandler.Unregister();
        }

        public void OnStartServer()
        {
        }

        public void OnStopServer()
        {
        }

        public void OnClientConnect(NetworkConnection net)
        {
            RegisterProtocol<ServerProtocolRegister>(net);
        }

        public void OnClientDisconnect(NetworkConnection net)
        {
            UnregisterProtocol(net);
        }

        private void RegisterProtocol<T>(NetworkConnection net) where T : IProtocolRegister, new()
        {
            if (!m_Registers.ContainsKey(net.connectionId))
            {
                IProtocolRegister register = new T();
                register.Register(net);
                m_Registers.Add(net.connectionId, register);
            }
        }
        private void UnregisterProtocol(NetworkConnection net)
        {
            IProtocolRegister register = null;
            if (m_Registers.TryGetValue(net.connectionId, out register))
            {
                register.Unregister(net);
                m_Registers.Remove(net.connectionId);
            }
        }
    }
}