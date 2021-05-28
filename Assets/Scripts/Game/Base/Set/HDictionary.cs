using System.Collections;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class HDictionary<TKey, TValue> : IEnumerable<TValue>
    {
        private List<TValue> m_List;
        private Dictionary<TKey, TValue> m_Dict;

        public HDictionary() : this(16)
        {
        }

        public HDictionary(int capacity)
        {
            m_List = new List<TValue>(capacity);
            m_Dict = new Dictionary<TKey, TValue>(capacity);
        }

        public void Add(TKey k, TValue v)
        {
            if (!m_Dict.ContainsKey(k))
            {
                m_List.Add(v);
                m_Dict.Add(k, v);
            }
        }

        public bool Remove(TKey k)
        {
            if (m_Dict.TryGetValue(k, out TValue v))
            {
                m_List.Remove(v);
                m_Dict.Remove(k);
                return true;
            }
            return false;
        }

        public bool ContainsKey(TKey k)
        {
            return m_Dict.ContainsKey(k);
        }

        public bool TryGetValue(TKey k, out TValue v)
        {
            return m_Dict.TryGetValue(k, out v);
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return new ListEnumerator<TValue>(m_List);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private struct ListEnumerator<TValue> : IEnumerator<TValue>
        {
            private int m_Index;
            private int m_Length;
            private List<TValue> m_List;

            public ListEnumerator(List<TValue> list)
            {
                m_Index = -1;
                m_List = list;
                m_Length = list.Count;
            }

            public bool MoveNext()
            {
                m_Index++;
                return m_Index < m_Length;
            }

            public void Reset()
            {
                m_Index = -1;
            }

            public object Current
            {
                get { return m_List[m_Index]; }
            }

            public void Dispose()
            {
                m_List = null;
            }

            TValue IEnumerator<TValue>.Current
            {
                get { return m_List[m_Index]; }
            }
        }
    }
}