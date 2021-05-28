using Nice.Game.Base;

namespace Nice.Game.Server
{
    public class ServerWorld : AbsWorld
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