using System;
using System.Collections.Generic;

namespace Zyq.Game.Base
{
    public class Entities : IDisposable, IUpdate, IFixedUpdate
    {
        private List<Entity> mTemp;
        private List<Entity> mEntityLts;
        private Dictionary<uint, Entity> mEntityDys;
        private Dictionary<uint, List<Entity>> mEntityGps;

        public Entities()
        {
            mTemp = new List<Entity>();
            mEntityLts = new List<Entity>();
            mEntityDys = new Dictionary<uint, Entity>();
            mEntityGps = new Dictionary<uint, List<Entity>>();
        }

        public void Dispose()
        {
            mTemp.Clear();
            mEntityLts.Clear();
            mEntityDys.Clear();
            mEntityGps.Clear();
        }

        public void OnUpdate(float delta)
        {
            Refill();
            if (mTemp.Count > 0)
            {
                for (int i = 0; i < mTemp.Count; ++i)
                {
                    mTemp[i].OnUpdate(delta);
                }
            }
        }

        public void OnFixedUpdate(float delta)
        {
            Refill();
            if (mTemp.Count > 0)
            {
                for (int i = 0; i < mTemp.Count; ++i)
                {
                    mTemp[i].OnFixedUpdate(delta);
                }
            }
        }

        public List<Entity> GetGpsEntitys(uint group)
        {
            List<Entity> entitys = null;
            if (!mEntityGps.TryGetValue(group, out entitys))
            {
                entitys = new List<Entity>();
                mEntityGps.Add(group, entitys);
            }
            return entitys;
        }

        public Entity AddEntity(Entity entity)
        {
            if (!mEntityDys.ContainsKey(entity.Eid))
            {
                mEntityLts.Add(entity);
                mEntityDys.Add(entity.Eid, entity);
                GetGpsEntitys(entity.Gid).Add(entity);
                entity.OnAdd();
                return entity;
            }
            return null;
        }

        public Entity RemoveEntity(uint eid)
        {
            Entity entity = null;
            if (mEntityDys.TryGetValue(eid, out entity))
            {
                mEntityLts.Remove(entity);
                GetGpsEntitys(entity.Gid).Remove(entity);
                mEntityDys.Remove(eid);
            }
            return entity;
        }

        public Entity GetEntity(uint eid)
        {
            Entity entity = null;
            mEntityDys.TryGetValue(eid, out entity);
            return entity;
        }

        public void Dispatcher(int id, uint eid, IBody body)
        {
            if (eid > 0)
            {
                Entity entity = null;
                if (mEntityDys.TryGetValue(eid, out entity))
                {
                    entity.Dispatcher(id, body);
                }
            }
            else
            {
                Refill();
                for (int i = 0; i < mTemp.Count; ++i)
                {
                    mTemp[i].Dispatcher(id, body);
                }
            }
        }

        private void Refill()
        {
            mTemp.Clear();
            mTemp.AddRange(mEntityLts);
        }

        public List<Entity> ALL { get { return mEntityLts; } }
    }
}