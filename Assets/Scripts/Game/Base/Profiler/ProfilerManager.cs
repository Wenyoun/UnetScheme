using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Rendering;

namespace Zyq.Game.Base
{
    public class ProfilerManager
    {
        private struct Sampler
        {
            public int Count;
            public float MinTime;
            public float MaxTime;
            public float TotalTime;
            public float Realtime;
            public bool IsEnter;

            public Sampler(string name)
            {
                Count = 0;
                MinTime = 999999999;
                MaxTime = -999999999;
                TotalTime = 0;
                Realtime = Time.realtimeSinceStartup;
                IsEnter = false;
            }

            public override string ToString()
            {
                return Count + "," + Convert(MinTime) + "," + Convert(MaxTime) + "," + Convert(TotalTime);
            }

            private float Convert(float time)
            {
                return ((int) (time * 1000 * 10000)) / 10000.0f;
            }
        }

        private static string full;
        private static Dictionary<string, Sampler> samplers;

        public static void Config(string path, string filename)
        {
            full = string.Concat(path, "/", filename);
            samplers = new Dictionary<string, Sampler>();
        }

        public static void BeginSample(string name)
        {
            if (samplers != null)
            {
                Sampler sampler;
                if (!samplers.TryGetValue(name, out sampler))
                {
                    sampler = new Sampler(name);
                    samplers.Add(name, sampler);
                }

                if (!sampler.IsEnter)
                {
                    sampler.IsEnter = true;
                    sampler.Realtime = Time.realtimeSinceStartup;
                }
            }
        }

        public static void EndSample(string name)
        {
            if (samplers != null)
            {
                if (samplers.TryGetValue(name, out Sampler sampler))
                {
                    if (sampler.IsEnter)
                    {
                        sampler.Count += 1;
                        sampler.IsEnter = false;
                        float time = Time.realtimeSinceStartup - sampler.Realtime;
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

                int length = keys.Count;
                int lastLength = length;
                for(int i=0; i<length; ++i)
                {
                    string key = keys[i];
                    Sampler sampler = samplers[key];
                    builder.Append(key).Append(",").Append(sampler.ToString());
                    if (i != lastLength)
                    {
                        builder.Append("\n");
                    }
                }

                if (File.Exists(full))
                {
                    File.Delete(full);
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