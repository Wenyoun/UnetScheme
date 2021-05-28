namespace Nice.Game.Server
{
    public partial class Server
    {
        public void AddServerLogic<T>() where T : IServerLogic, new()
        {
            m_LogicManager.AddServerLogic<T>(this);
        }

        public void RemoveServerLogic<T>() where T : IServerLogic
        {
            m_LogicManager.RemoveServerLogic<T>();
        }

        public T GetServerLogic<T>() where T : IServerLogic
        {
            return m_LogicManager.GetServerLogic<T>();
        }
    }
}