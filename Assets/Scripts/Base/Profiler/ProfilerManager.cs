using System;
using System.Collections.Generic;

namespace Base
{
    public class ProfilerManager
    {
        private class Sampler
        {
            public string Name;
            public int Count;
            public double MinTime;
            public double MaxTime;
            public double TotalTime;
            public DateTime Timestamp;
            public bool IsEnter;

            public Sampler(string name)
            {
                Name = name;
                Count = 0;
                MinTime = 999999999;
                MaxTime = -999999999;
                TotalTime = 0;
                IsEnter = false;
            }

            public override string ToString()
            {
                return Name + ":" + Count + ":" + MinTime + ":" + MaxTime + ":" + TotalTime;
            }
        }

        private static Dictionary<string, Sampler> profilers = new Dictionary<string, Sampler>();

        public static void Begin(string name)
        {
            Sampler sampler = null;
            if (!profilers.TryGetValue(name, out sampler))
            {
                sampler = new Sampler(name);
                profilers.Add(name, sampler);
            }

            if (!sampler.IsEnter)
            {
                sampler.IsEnter = true;
                sampler.Timestamp = DateTime.Now;
            }
        }

        public static void End(string name)
        {
            Sampler sampler = null;
            if (profilers.TryGetValue(name, out sampler))
            {
                if (sampler.IsEnter)
                {
                    sampler.Count += 1;
                    sampler.IsEnter = false;
                    double time = (DateTime.Now - sampler.Timestamp).TotalMilliseconds;
                    if (sampler.MinTime > time)
                    {
                        sampler.MinTime = time;
                    }
                    if (sampler.MaxTime < time)
                    {
                        sampler.MaxTime = time;
                    }
                    sampler.TotalTime += time;
                }
            }
        }
    }
}