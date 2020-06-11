using UnityEngine;

namespace Zyq.Game.Base
{
    public class QuaternionBody : IBody
    {
        public Quaternion Value;

        public QuaternionBody Init(Quaternion value)
        {
            Value = value;
            return this;
        }

        public static QuaternionBody Default = new QuaternionBody();
    }
}