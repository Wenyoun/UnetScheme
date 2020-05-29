using UnityEngine.Networking;

using Zyq.Game.Base;

using System.Collections.Generic;

namespace Zyq.Game.Client
{
    public class Client
    {
        public static Client Ins = new Client();

        private Dictionary<System.Type, IProtocolRegister> m_Registers;

        public void Init()
        {
            m_Registers = new Dictionary<System.Type, IProtocolRegister>();
            ObjectRegisterHandler.Register();
        }

        public void Dispose()
        {
            Net = null;
            m_Registers.Clear();
            m_Registers = null;
            ObjectRegisterHandler.Unregister();
        }

        public void OnStartClient()
        {
        }

        public void OnStopClient()
        {
        }

        public void OnServerConnect(NetworkConnection net)
        {
            Net = net;
            RegisterProtocol<ClientProtocolRegister>(Net);
        }

        public void OnServerDisconnect(NetworkConnection net)
        {
            foreach (IProtocolRegister register in m_Registers.Values)
            {
                register.Unregister(net);
            }
            m_Registers.Clear();
        }

        private void RegisterProtocol<T>(NetworkConnection net) where T : IProtocolRegister, new()
        {
            System.Type type = typeof(T);
            if (!m_Registers.ContainsKey(type))
            {
                IProtocolRegister register = new T();
                register.Register(net);
                m_Registers.Add(type, register);
            }
        }

        public NetworkConnection Net { get; private set; }
    }
}