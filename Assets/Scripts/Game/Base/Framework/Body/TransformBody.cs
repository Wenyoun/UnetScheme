using UnityEngine;

namespace Nice.Game.Base
{
    public class TransformBody : IBody
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public TransformBody Init(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            return this;
        }

        public static TransformBody Default = new TransformBody();
    }
}