using UnityEngine;
using System.Collections.Generic;

namespace Base
{
    public class AudioMgr : AbsCompose
    {
        public static AudioMgr Ins;

        private Audio mBg;
        private GameObject mRoot;
        private List<Audio> mTemps;
        private List<Audio> mBusys;
        private Queue<Audio> mIdles;

        public AudioMgr()
        {
            mTemps = new List<Audio>();
            mBusys = new List<Audio>();
            mIdles = new Queue<Audio>();
        }

        public override void OnInit()
        {
            Ins = this;
            mRoot = new GameObject("Audios");
            mRoot.AddComponent<AudioListener>();
            mBg = Create();
            for (int i = 1; i < 10; ++i)
            {
                mIdles.Enqueue(Create());
            }
            mRoot.transform.SetAsFirstSibling();
            GameObject.DontDestroyOnLoad(mRoot);
        }

        public override void OnRemove()
        {
            mTemps.Clear();
            mIdles.Clear();
            mBusys.Clear();
            Object.Destroy(mRoot);
            mBg = null;
            mRoot = null;
            Ins = null;
        }

        public override void OnUpdate(float delta)
        {
            int count = mBusys.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    Audio audio = mBusys[i];
                    if (audio.Target != null)
                    {
                        if (!audio.Target.isPlaying)
                        {
                            mTemps.Add(audio);
                        }
                    }
                    else
                    {
                        mTemps.Add(audio);
                    }
                }

                if (mTemps.Count > 0)
                {
                    for (int i = 0; i < mTemps.Count; ++i)
                    {
                        Audio audio = mTemps[i];
                        mBusys.Remove(audio);
                        audio.Stop();
                        mIdles.Enqueue(audio);
                    }
                    mTemps.Clear();
                }
            }
        }

        public void Play(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Play(Resources.Load<AudioClip>(path), 1);
            }
        }


        public void Play(AudioClip clip, float volume)
        {
            Audio audio = null;
            if (mIdles.Count > 0)
            {
                audio = mIdles.Dequeue();
            }
            else
            {
                audio = Create();
            }
            audio.Play(clip, false, volume);
            mBusys.Add(audio);
        }

        public void PlayBg(AudioClip clip, float volume)
        {
            if (mBg != null)
            {
                mBg.Play(clip, true, volume);
            }
        }

        public void StopBg()
        {
            if (mBg != null)
            {
                mBg.Stop();
            }
        }

        private Audio Create()
        {
            GameObject go = new GameObject("Audio");
            go.transform.SetParent(mRoot.transform);
            AudioSource target = go.AddComponent<AudioSource>();
            target.clip = null;
            target.playOnAwake = false;
            return new Audio(target);
        }

        private class Audio
        {
            public int Code;
            public AudioSource Target;

            public Audio(AudioSource target)
            {
                Target = target;
            }

            public void Play(AudioClip clip, bool loop, float volume)
            {
                if (Target != null)
                {
                    Code = clip.GetHashCode();
                    Target.clip = clip;
                    Target.loop = loop;
                    Target.volume = volume;
                    Target.Play();
                }
            }

            public void Stop()
            {
                if (Target != null)
                {
                    Target.Stop();
                    Target.clip = null;
                    Target.loop = false;
                }
            }
        }
    }
}