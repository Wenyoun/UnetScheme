using System;

namespace Zyq.Game.Base
{
    public interface IWorld : IDisposable, IUpdate, ILateUpdate, IFixedUpdate
    {
        int WorldId { get; }

        void AddEntity(Entity entity);

        void RemoveEntity(uint entityId);

        Entity GetEntity(uint entityId);

        void Dispatcher(int msgId);

        void Dispatcher(int msgId, IBody body);

        void Dispatcher(int msgId, uint entityId);

        void Dispatcher(int msgId, uint entityId, IBody body);

        TimerMgr WorldTimer { get; }

        UpdateMgr WorldUpdate { get; }

        MessageMgr WorldMessage { get; }
    }
}