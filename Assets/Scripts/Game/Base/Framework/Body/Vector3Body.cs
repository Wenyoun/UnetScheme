using UnityEngine;

namespace Nice.Game.Base
{
    public class Vector3Body : IBody
    {
        public Vector3 Value;

        public Vector3Body Init(Vector3 value)
        {
            Value = value;
            return this;
        }

        public static Vector3Body Default = new Vector3Body();
    }
}