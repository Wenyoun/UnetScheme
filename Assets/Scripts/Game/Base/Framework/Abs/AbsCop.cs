using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public abstract class AbsCop : ICop
    {
        private uint m_CopId;
        private IEntity m_Entity;
        private List<UpdateDelegate> m_Updates;
        private List<UpdateDelegate> m_FixedUpdates;
        private Dictionary<int, MsgDelegate> m_Msssages;

        public AbsCop()
        {
            m_CopId = UniGenID.GenNextCopID();
            m_Updates = new List<UpdateDelegate>();
            m_FixedUpdates = new List<UpdateDelegate>();
            m_Msssages = new Dictionary<int, MsgDelegate>();
        }

        public abstract void OnInit();

        public virtual void OnRemove()
        {
            Dictionary<int, MsgDelegate>.Enumerator it = m_Msssages.GetEnumerator();
            while (it.MoveNext())
            {
                KeyValuePair<int, MsgDelegate> pair = it.Current;
                m_Entity.Message.Unregister(pair.Key, pair.Value);
            }

            int length = m_Updates.Count;
            for (int i = 0; i < length; ++i)
            {
                m_Entity.Update.UnregisterUpdate(m_Updates[i]);
                m_Entity.Update.UnregisterFixedUpdate(m_FixedUpdates[i]);
            }

            m_Updates.Clear();
            m_Msssages.Clear();
            m_FixedUpdates.Clear();
        }

        public void RegisterMessage(int id, MsgDelegate handler)
        {
            if (!m_Msssages.ContainsKey(id))
            {
                m_Msssages.Add(id, handler);
                m_Entity.Message.Register(id, handler);
            }
        }

        public void UnregisterMessage(int id)
        {
            if (m_Msssages.TryGetValue(id, out MsgDelegate handler))
            {
                m_Msssages.Remove(id);
                m_Entity.Message.Unregister(id, handler);
            }
        }

        public void RegisterUpdate(UpdateDelegate update)
        {
            if (!m_Updates.Contains(update))
            {
                m_Updates.Add(update);
                m_Entity.Update.RegisterUpdate(update);
            }
        }

        public void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (!m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Add(fixedUpdate);
                m_Entity.Update.RegisterFixedUpdate(fixedUpdate);
            }
        }

        public void UnregisterUpdate(UpdateDelegate update)
        {
            if (m_Updates.Contains(update))
            {
                m_Updates.Remove(update);
                m_Entity.Update.UnregisterUpdate(update);
            }
        }

        public void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            if (m_FixedUpdates.Contains(fixedUpdate))
            {
                m_FixedUpdates.Remove(fixedUpdate);
                m_Entity.Update.UnregisterFixedUpdate(fixedUpdate);
            }
        }

        public int RegisterTimer(float delay, Action func)
        {
            return m_Entity.Timer.Register(delay, func);
        }

        public int RegisterTimer(float delay, float interval, int count, Action func, Action finish = null)
        {
            return m_Entity.Timer.Register(delay, interval, count, func, finish);
        }

        public void UnregisterTimer(int id)
        {
            m_Entity.Timer.Unregister(id);
        }

        public uint CopId
        {
            get { return m_CopId; }
        }

        public IEntity Entity
        {
            get { return m_Entity; }
            set { m_Entity = value; }
        }

        public T CastEntity<T>() where T : IEntity
        {
            return (T) m_Entity;
        }
    }
}