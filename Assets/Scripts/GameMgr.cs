using Base;
using System;
using Nice.Game.Base;
using System.Collections.Generic;

public class GameMgr : ILifecycle, IUpdate, ILateUpdate, IFixedUpdate
{
    public static GameMgr Ins = new GameMgr();
    private bool m_Dispose;
    private List<ICompose> m_Composes;

    private GameMgr()
    {
        m_Composes = new List<ICompose>();
    }

    public void Config()
    {
        SettingMgr.Config();
        GlobalMgr.Config();
        ProfilerManager.Config("/Users/yinhuayong/Desktop", "test.txt");
    }

    public void OnInit()
    {
        m_Dispose = false;
        Add<AudioMgr>();
        Add<SystemBehaviour>();
    }

    public void OnRemove()
    {
        if (m_Dispose)
        {
            return;
        }
        m_Dispose = true;
        int length = m_Composes.Count;
        for (int i = 0; i < length; ++i)
        {
            m_Composes[i].OnRemove();
        }
        m_Composes.Clear();
        ProfilerManager.Dispose();
    }

    public void OnUpdate(float delta)
    {
        if (m_Dispose)
        {
            return;
        }
        int length = m_Composes.Count;
        for (int i = 0; i < length; ++i)
        {
            m_Composes[i].OnUpdate(delta);
        }
    }

    public void OnLateUpdate(float delta)
    {
        if (m_Dispose)
        {
            return;
        }
        int length = m_Composes.Count;
        for (int i = 0; i < length; ++i)
        {
            m_Composes[i].OnLateUpdate(delta);
        }
    }

    public void OnFixedUpdate(float delta)
    {
        if (m_Dispose)
        {
            return;
        }
        int length = m_Composes.Count;
        for (int i = 0; i < length; ++i)
        {
            m_Composes[i].OnFixedUpdate(delta);
        }
    }

    public T Add<T>() where T : ICompose, new()
    {
        if (m_Dispose)
        {
            return default(T);
        }
        ICompose compose = Find<T>();
        if (compose == null)
        {
            compose = new T();
            m_Composes.Add(compose);
            compose.OnInit();
        }
        return (T) compose;
    }

    public void Remove<T>() where T : ICompose
    {
        if (m_Dispose)
        {
            return;
        }
        ICompose compose = Find<T>();
        if (compose != null)
        {
            m_Composes.Remove(compose);
            compose.OnRemove();
        }
    }

    private ICompose Find<T>() where T : ICompose
    {
        Type t = typeof(T);
        ICompose compose = null;
        int length = m_Composes.Count;
        for (int i = 0; i < length; ++i)
        {
            ICompose c = m_Composes[i];
            if (c.GetType() == t)
            {
                compose = c;
                break;
            }
        }
        return compose;
    }
}