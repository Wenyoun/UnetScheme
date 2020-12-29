using System;
using System.Collections.Generic;

namespace Nice.Game.Base
{
    public class TimerRegister : IDisposable, IUpdate
    {
        private static int StartIndex = 1;
        private List<int> m_Removes;
        private List<Wrapper> m_List;
        private List<Wrapper> m_Adds;

        public TimerRegister()
        {
            m_Removes = new List<int>(16);
            m_List = new List<Wrapper>(64);
            m_Adds = new List<Wrapper>(16);
        }

        public void Dispose()
        {
            m_List.Clear();
            m_Adds.Clear();
            m_Removes.Clear();
        }

        public int Register(float delay, Action func)
        {
            return Register(delay, delay, 1, func, null);
        }

        public int Register(float delay, float interval, int count, Action repeat, Action finish = null)
        {
            if (repeat != null)
            {
                Wrapper tick = new Wrapper();
                tick.Id = StartIndex++;
                tick.Time = delay;
                tick.Interval = interval;
                tick.Count = count < 0 ? int.MaxValue : count;
                tick.Repeat = repeat;
                tick.Finish = finish;
                tick.NextFrame = null;
                tick.IsRemove = false;
                m_Adds.Add(tick);
                return tick.Id;
            }
            return -1;
        }

        public int RegisterNextFrame(Action frame)
        {
            if (frame != null)
            {
                Wrapper tick = new Wrapper();
                tick.Id = StartIndex++;
                tick.Time = 0;
                tick.Interval = 0;
                tick.Count = 0;
                tick.Repeat = null;
                tick.Finish = null;
                tick.NextFrame = frame;
                tick.IsRemove = false;
                m_Adds.Add(tick);
                return tick.Id;
            }
            return -1;
        }

        public void UnRegister(int id)
        {
            TryRemove(id);
        }

        public void OnUpdate(float delta)
        {
            if (m_Adds.Count > 0)
            {
                m_List.AddRange(m_Adds);
                m_Adds.Clear();
            }

            int length = m_List.Count;
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                {
                    Wrapper tick = m_List[i];

                    if (tick.IsRemove)
                    {
                        m_Removes.Add(tick.Id);
                        continue;
                    }

                    if (tick.NextFrame != null)
                    {
                        tick.NextFrame.Invoke();
                        m_Removes.Add(tick.Id);
                        continue;
                    }

                    tick.Time -= delta;

                    if (tick.Time <= 0)
                    {
                        if (tick.Repeat != null)
                        {
                            tick.Repeat.Invoke();
                        }

                        tick.Count -= 1;

                        if (tick.Count <= 0)
                        {
                            if (tick.Finish != null)
                            {
                                tick.Finish();
                            }

                            m_Removes.Add(tick.Id);
                            continue;
                        }

                        tick.Time = tick.Interval;
                    }

                    m_List[i] = tick;
                }

                length = m_Removes.Count;
                if (length > 0)
                {
                    for (int i = 0; i < length; ++i)
                    {
                        int id = m_Removes[i];
                        int index = SearchIndex(id);
                        if (index >= 0)
                        {
                            m_List.RemoveAt(index);
                        }
                    }
                    m_Removes.Clear();
                }
            }
        }

        private void TryRemove(int id)
        {
            if (id > 0)
            {
                int index = SearchIndex(id);
                if (index >= 0)
                {
                    Wrapper wp = m_List[index];
                    wp.IsRemove = true;
                    m_List[index] = wp;
                }
            }
        }

        private int SearchIndex(int id)
        {
            int index = -1;
            int length = m_List.Count;
            for (int i = 0; i < length; ++i)
            {
                if (m_List[i].IsEquals(id))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private struct Wrapper
        {
            public int Id;
            public int Count;
            public float Time;
            public float Interval;
            public Action Repeat;
            public Action Finish;
            public Action NextFrame;
            public bool IsRemove;

            public bool IsEquals(int id)
            {
                return Id == id;
            }
        }
    }
}