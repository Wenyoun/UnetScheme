using Base;
using System;
using System.Collections.Generic;

public class GameMgr : ILifecycle, IUpdate, IFixedUpdate
{
    public static GameMgr Ins = new GameMgr();

    private List<ICompose> mComposeLts;
    private Dictionary<System.Type, ICompose> mComposeDys;

    public GameMgr()
    {
        mComposeLts = new List<ICompose>();
        mComposeDys = new Dictionary<System.Type, ICompose>();
    }

    public void Config()
    {
        SettingMgr.Config();
        GlobalMgr.Config();
    }

    public void OnInit()
    {
        Add<AudioMgr>();
        Add<SystemBehaviour>();
    }

    public void OnRemove()
    {
        for (int i = mComposeLts.Count - 1; i > 0; --i)
        {
            mComposeLts[i].OnRemove();
        }
        mComposeLts.Clear();
        mComposeDys.Clear();
    }

    public void OnUpdate(float delta)
    {
        for (int i = 0; i < mComposeLts.Count; ++i)
        {
            mComposeLts[i].OnUpdate(delta);
        }
    }

    public void OnFixedUpdate(float delta)
    {
        for (int i = 0; i < mComposeLts.Count; ++i)
        {
            mComposeLts[i].OnFixedUpdate(delta);
        }
    }

    public T Add<T>() where T : ICompose, new()
    {
        System.Type type = typeof(T);
        if (!mComposeDys.ContainsKey(type))
        {
            T compose = new T();
            mComposeLts.Add(compose);
            mComposeDys.Add(type, compose);
            compose.OnInit();
            return compose;
        }
        return default(T);
    }

    public void Remove<T>() where T : ICompose
    {
        System.Type type = typeof(T);
        ICompose compose = null;
        if (mComposeDys.TryGetValue(type, out compose))
        {
            mComposeLts.Remove(compose);
            mComposeDys.Remove(type);
            compose.OnRemove();
        }
    }
}