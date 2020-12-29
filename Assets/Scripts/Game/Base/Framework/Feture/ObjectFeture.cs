using UnityEngine;

namespace Nice.Game.Base
{
    public abstract class ObjectFeture : IFeture
    {
        public abstract Vector3 scale { get; set; }

        public abstract Vector3 position { get; set; }

        public abstract Quaternion rotation { get; set; }
    }
}