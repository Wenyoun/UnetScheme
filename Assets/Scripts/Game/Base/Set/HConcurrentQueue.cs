using System.Collections.Concurrent;

namespace Nice.Game.Base
{
    public class HConcurrentQueue<T>
    {
        private ConcurrentQueue<T> m_Queue;

        public HConcurrentQueue()
        {
            m_Queue = new ConcurrentQueue<T>();
        }

        public bool IsEmpty
        {
            get { return m_Queue.IsEmpty; }
        }

        public int Count
        {
            get { return m_Queue.Count; }
        }

        public void Enqueue(T t)
        {
            m_Queue.Enqueue(t);
        }

        public bool TryDequeue(out T t)
        {
            return m_Queue.TryDequeue(out t);
        }

        public bool TryPeek(out T t)
        {
            return m_Queue.TryPeek(out t);
        }

        public void Clear()
        {
            while (m_Queue.TryDequeue(out T t))
            {
            }
        }
    }
}