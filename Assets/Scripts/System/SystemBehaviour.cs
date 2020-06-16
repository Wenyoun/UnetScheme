using Base;

namespace System
{
    public sealed class SystemBehaviour : AbsBehaviour
    {
        public override void OnInit()
        {
            Add<SystemViewModule>();
        }
    }
}