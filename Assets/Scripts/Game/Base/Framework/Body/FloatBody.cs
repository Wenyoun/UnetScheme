namespace Nice.Game.Base
{
    public class FloatBody : IBody
    {
        public float Value;

        public FloatBody Init(float value)
        {
            Value = value;
            return this;
        }

        public static FloatBody Default = new FloatBody();
    }
}