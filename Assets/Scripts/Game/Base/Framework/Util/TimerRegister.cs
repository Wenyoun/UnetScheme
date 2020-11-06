using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class TimerRegister : IDisposable, IUpdate
    {
        private static int ID = 1;
        private bool m_IsInitialize;
        private List<int> m_Temp;
        private List<TimerTick> m_List;
        private List<TimerTick> m_Adds;

        public TimerRegister()
        {
            m_Temp = new List<int>();
            m_List = new List<TimerTick>(100);
            m_Adds = new List<TimerTick>(10);
            m_IsInitialize = true;
        }

        public void Dispose()
        {
            m_IsInitialize = false;
            m_Temp.Clear();
            m_List.Clear();
            m_Adds.Clear();
        }

        public int Register(float delay, Action func)
        {
            if (m_IsInitialize)
            {
                return Register(delay, delay, 1, func, null);
            }

            return -1;
        }

        public int Register(float delay, float interval, int count, Action func, Action finish = null)
        {
            if (m_IsInitialize && func != null)
            {
                TimerTick tick = new TimerTick();
                tick.Id = ID++;
                tick.Time = delay;
                tick.Interval = interval;
                tick.Count = count < 0 ? int.MaxValue : count;
                tick.Func = func;
                tick.Finish = finish;
                tick.IsRemove = false;
                m_Adds.Add(tick);
                return tick.Id;
            }

            return -1;
        }

        public int RegisterFrame(Action frame)
        {
            if (m_IsInitialize && frame != null)
            {
                TimerTick tick = new TimerTick();
                tick.Id = ID++;
                tick.Time = 0;
                tick.Interval = 0;
                tick.Count = 0;
                tick.Func = null;
                tick.Finish = null;
                tick.Frame = frame;
                tick.IsRemove = false;
                m_Adds.Add(tick);
                return tick.Id;
            }

            return -1;
        }

        public void Unregister(int id)
        {
            if (m_IsInitialize)
            {
                TryRemove(id);
            }
        }

        public void OnUpdate(float delta)
        {
            if (m_IsInitialize)
            {
                if (m_Adds.Count > 0)
                {
                    m_List.AddRange(m_Adds);
                    m_Adds.Clear();
                }

                if (delta > 0 && m_List.Count > 0)
                {
                    for (int i = 0; i < m_List.Count; ++i)
                    {
                        TimerTick tick = m_List[i];

                        if (tick.IsRemove)
                        {
                            m_Temp.Add(tick.Id);
                            continue;
                        }

                        if (tick.Frame != null)
                        {
                            tick.Frame();
                            continue;
                        }

                        tick.Time -= delta;

                        if (tick.Time <= 0)
                        {
                            if (tick.Func != null)
                            {
                                tick.Func();
                            }

                            tick.Count -= 1;

                            if (tick.Count <= 0)
                            {
                                m_Temp.Add(tick.Id);

                                if (tick.Finish != null)
                                {
                                    tick.Finish();
                                }
                            }
                            else
                            {
                                tick.Time = tick.Interval;
                            }
                        }
                    }

                    if (m_Temp.Count > 0)
                    {
                        for (int i = 0; i < m_Temp.Count; ++i)
                        {
                            int index = -1;
                            int id = m_Temp[i];

                            for (int j = 0; j < m_List.Count; ++j)
                            {
                                if (id == m_List[j].Id)
                                {
                                    index = j;
                                }
                            }

                            if (index >= 0)
                            {
                                m_List.RemoveAt(index);
                            }
                        }

                        m_Temp.Clear();
                    }
                }
            }
        }

        private bool TryRemove(int id)
        {
            if (m_IsInitialize)
            {
                for (int i = 0; i < m_List.Count; ++i)
                {
                    TimerTick tick = m_List[i];
                    if (tick.Id == id)
                    {
                        tick.IsRemove = true;
                        m_List[i] = tick;
                        return true;
                    }
                }
            }

            return false;
        }

        private struct TimerTick
        {
            public int Id;
            public int Count;
            public float Time;
            public float Interval;
            public System.Action Func;
            public System.Action Frame;
            public System.Action Finish;
            public bool IsRemove;

            public void Reset()
            {
                Id = 0;
                Count = 0;
                Time = 0;
                Interval = 0;
                Func = null;
                Frame = null;
                Finish = null;
                IsRemove = false;
            }
        }
    }
}