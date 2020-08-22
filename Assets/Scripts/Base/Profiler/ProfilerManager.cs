using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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
                return Name + "," + Count + "," + MinTime + "," + MaxTime + "," + TotalTime;
            }
        }

        private static string full;
        private static Dictionary<string, Sampler> samplers;
        
        public static void Config(string path, string filename)
        {
            full = string.Concat(path, "/", filename);
            samplers = new Dictionary<string, Sampler>();
        }

        public static void Begin(string name)
        {
            if (samplers != null)
            {
                Sampler sampler = null;
                if (!samplers.TryGetValue(name, out sampler))
                {
                    sampler = new Sampler(name);
                    samplers.Add(name, sampler);
                }

                if (!sampler.IsEnter)
                {
                    sampler.IsEnter = true;
                    sampler.Timestamp = DateTime.Now;
                }
            }
        }

        public static void End(string name)
        {
            if (samplers != null)
            {
                Sampler sampler = null;
                if (samplers.TryGetValue(name, out sampler))
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

        public static void Dispose()
        {
            if (full != null && samplers != null && samplers.Count > 0)
            {
                DateTime start = DateTime.Now;
                
                List<string> keys = new List<string>(samplers.Keys);
                keys.Sort();

                StringBuilder builder = new StringBuilder();

                foreach (string key in keys)
                {
                    Sampler sampler = samplers[key];
                    if (sampler.Count > 1)
                    {
                        builder.Append(sampler.ToString() + "\n");
                    }
                }

                using (FileStream stream = File.Open(full, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(builder.ToString());
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                    stream.Close();
                }

                
                DateTime end = DateTime.Now;
                
                Debug.Log("save path = " + full + ",write time = " + (end - start).Milliseconds);
                
                samplers.Clear();
                full = null;
                samplers = null;
            }
        }
    }
}