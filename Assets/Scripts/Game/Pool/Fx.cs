using UnityEngine;
using System.Collections;

namespace Game
{
    public class Fx : MonoBehaviour
    {
        [HideInInspector] public string Path;

        private float m_Duration;
        private ParticleSystem[] m_Particles;

        private void Awake()
        {
            m_Particles = GetComponentsInChildren<ParticleSystem>();
            m_Duration = GetMaxDuration();
            Stop();
        }

        public void Play(bool shot = false)
        {
            for (int i = 0; i < m_Particles.Length; ++i)
            {
                ParticleSystem particle = m_Particles[i];
                if (!particle.isPlaying)
                {
                    particle.Play();
                }
            }

            if (shot)
            {
                StartCoroutine(OnRecycle());
            }
        }

        public void Stop()
        {
            for (int i = 0; i < m_Particles.Length; ++i)
            {
                ParticleSystem particle = m_Particles[i];
                if (particle.isPlaying)
                {
                    particle.Stop();
                }
            }
        }

        private float GetMaxDuration()
        {
            float max = 0;
            for (int i = 0; i < m_Particles.Length; ++i)
            {
                ParticleSystem particle = m_Particles[i];
                if (particle.main.duration > max)
                {
                    max = particle.main.duration;
                }
            }
            return max;
        }

        private IEnumerator OnRecycle()
        {
            yield return new WaitForSeconds(m_Duration);
            Stop();
            CachedMgr.Fx.Recycle(this);
        }
    }
}