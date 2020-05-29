using Base;
using System;
using System.Collections.Generic;

namespace Game
{
    public class TimerRegister : IDisposable, IUpdate
    {
        private static int ID = 1;

        private bool m_IsInitialize;

        private List<TimerTick> m_Temp;
        private List<TimerTick> m_List;
        private List<TimerTick> m_Adds;

        public TimerRegister()
        {
            m_Temp = new List<TimerTick>();
            m_List = new List<TimerTick>();
            m_Adds = new List<TimerTick>();

            OnInit();
        }

        public void OnInit()
        {
            Clear();
            m_IsInitialize = true;
        }

        public void Dispose()
        {
            Clear();
            m_IsInitialize = false;
        }

        public void Clear()
        {
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
                tick.Remove = false;
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
                tick.Remove = false;
                m_Adds.Add(tick);
                return tick.Id;
            }
            return -1;
        }

        public void Unregister(int id)
        {
            if (m_IsInitialize)
            {
                TimerTick tick = Try(id);
                if (tick != null)
                {
                    tick.Remove = true;
                }
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

                        if (tick.Remove)
                        {
                            m_Temp.Add(tick);
                        }
                        else
                        {
                            if (tick.Frame != null)
                            {
                                tick.Frame();
                            }
                            else
                            {
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
                                        m_Temp.Add(tick);
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
                        }
                    }

                    if (m_Temp.Count > 0)
                    {
                        for (int i = 0; i < m_Temp.Count; ++i)
                        {
                            TimerTick tick = m_Temp[i];
                            m_List.Remove(tick);
                        }
                        m_Temp.Clear();
                    }
                }
            }
        }

        private TimerTick Try(int id)
        {
            if (m_IsInitialize)
            {
                for (int i = 0; i < m_List.Count; ++i)
                {
                    TimerTick tick = m_List[i];
                    if (tick.Id == id)
                    {
                        return tick;
                    }
                }
            }
            return null;
        }
    }
}