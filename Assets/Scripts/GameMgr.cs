using Base;
using System;
using Zyq.Game.Base;
using System.Collections.Generic;

public class GameMgr : ILifecycle, IUpdate, ILateUpdate, IFixedUpdate
{
    public static GameMgr Ins = new GameMgr();

    private List<ICompose> mComposeLts;
    private Dictionary<Type, ICompose> mComposeDys;

    public GameMgr()
    {
        mComposeLts = new List<ICompose>();
        mComposeDys = new Dictionary<Type, ICompose>();
    }

    public void Config()
    {
        SettingMgr.Config();
        GlobalMgr.Config();
        ProfilerManager.Config("/Users/yinhuayong/Desktop", "test.txt");
    }

    public void OnInit()
    {
        Add<AudioMgr>();
        Add<SystemBehaviour>();
    }

    public void OnRemove()
    {
        int length = mComposeLts.Count;
        for (int i = 0; i < length; ++i)
        {
            mComposeLts[i].OnRemove();
        }
        mComposeLts.Clear();
        mComposeDys.Clear();
        ProfilerManager.Dispose();
    }

    public void OnUpdate(float delta)
    {
        int length = mComposeLts.Count;
        for (int i = 0; i < length; ++i)
        {
            mComposeLts[i].OnUpdate(delta);
        }
    }

    public void OnLateUpdate()
    {
        int length = mComposeLts.Count;
        for (int i = 0; i < length; ++i)
        {
            mComposeLts[i].OnLateUpdate();
        }
    }

    public void OnFixedUpdate(float delta)
    {
        int length = mComposeLts.Count;
        for (int i = 0; i < length; ++i)
        {
            mComposeLts[i].OnFixedUpdate(delta);
        }
    }

    public T Add<T>() where T : ICompose, new()
    {
        Type type = typeof(T);
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
        Type type = typeof(T);
        ICompose compose;
        if (mComposeDys.TryGetValue(type, out compose))
        {
            mComposeLts.Remove(compose);
            mComposeDys.Remove(type);
            compose.OnRemove();
        }
    }
}