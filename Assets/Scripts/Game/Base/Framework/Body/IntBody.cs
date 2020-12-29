namespace Nice.Game.Base
{
    public class IntBody : IBody
    {
        public int Value;

        public IntBody Init(int value)
        {
            Value = value;
            return this;
        }

        public static IntBody Default = new IntBody();
    }
}