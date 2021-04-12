using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerWorld : World
    {
        public ServerWorld() : base(1)
        {
        }

        protected override void Init()
        {
            AddFeature<SyncAttributeFeature>();
        }
    }
}