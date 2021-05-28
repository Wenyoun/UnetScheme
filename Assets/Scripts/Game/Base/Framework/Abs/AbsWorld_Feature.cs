namespace Nice.Game.Base
{
    public abstract partial class AbsWorld
    {
        public void AddFeature<T>() where T : IWorldFeature, new()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Features.AddFeature<T>();
        }

        public void RemoveFeature<T>() where T : IWorldFeature, new()
        {
            if (m_Dispose)
            {
                return;
            }
            m_Features.RemoveFeature<T>();
        }
    }
}