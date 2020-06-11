using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public abstract class AbsCop : ICop
    {
        private List<UpdateDelegate> m_Updates;
        private List<UpdateDelegate> m_FixedUpdates;
        private Dictionary<int, List<MsgDelegate>> m_Msssages;

        public AbsCop()
        {
            m_Updates = new List<UpdateDelegate>();
            m_FixedUpdates = new List<UpdateDelegate>();
            m_Msssages = new Dictionary<int, List<MsgDelegate>>();
        }

        public abstract void OnInit();

        public virtual void OnRemove()
        {
            Dictionary<int, List<MsgDelegate>>.Enumerator it = m_Msssages.GetEnumerator();
            while (it.MoveNext())
            {
                int id = it.Current.Key;
                List<MsgDelegate> list = it.Current.Value;
                for (int i = 0; i < list.Count; ++i)
                {
                    Entity.MsgRegister.Unregister(id, list[i]);
                }
            }

            for (int i = 0; i < m_Updates.Count; ++i)
            {
                Entity.UpdateRegister.UnregisterUpdate(m_Updates[i]);
            }

            for (int i = 0; i < m_FixedUpdates.Count; ++i)
            {
                Entity.UpdateRegister.UnregisterFixedUpdate(m_FixedUpdates[i]);
            }

            m_Msssages.Clear();
            m_Updates.Clear();
            m_FixedUpdates.Clear();
        }

        public void RegisterMessage(int id, MsgDelegate handler)
        {
            List<MsgDelegate> list = null;
            if (!m_Msssages.TryGetValue(id, out list))
            {
                list = new List<MsgDelegate>();
                m_Msssages.Add(id, list);
            }

            if (!list.Contains(handler))
            {
                list.Add(handler);
                Entity.MsgRegister.Register(id, handler);
            }
        }

        public void UnregisterMessage(int id, MsgDelegate handler)
        {
            List<MsgDelegate> list = null;
            if (m_Msssages.TryGetValue(id, out list))
            {
                if (list.Contains(handler))
                {
                    list.Remove(handler);
                    Entity.MsgRegister.Unregister(id, handler);
                }
            }
        }

        public void RegisterUpdate(UpdateDelegate update)
        {
            if (!m_Updates.Contains(update))
            {
                m_Updates.Add(update);
                Entity.UpdateRegister.RegisterUpdate(update);
            }
        }

        public void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (!m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Add(fixedUpdate);
                Entity.UpdateRegister.RegisterFixedUpdate(fixedUpdate);
            }
        }

        public void UnregisterUpdate(UpdateDelegate update)
        {
            if (m_Updates.Contains(update))
            {
                m_Updates.Remove(update);
                Entity.UpdateRegister.UnregisterUpdate(update);
            }
        }

        public void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Remove(fixedUpdate);
                Entity.UpdateRegister.UnregisterFixedUpdate(fixedUpdate);
            }
        }

        public IEntity Entity { get; set; }
    }
}