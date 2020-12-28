using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zyq.Game.Base
{
    public interface IWorldLogic
    {
        void OnInit(IWorld world);

        void OnRemove();
    }

    public abstract class AbsWorldLogic : IWorldLogic
    {
        private IWorld m_World;

        public void OnInit(IWorld world)
        {
            m_World = world;
            Init();
        }

        public void OnRemove()
        {
            Clear();
            m_World = null;
        }

        public IWorld World
        {
            get { return m_World; }
        }

        protected virtual void Init()
        {
        }

        protected virtual void Clear()
        {
        }
    }
}