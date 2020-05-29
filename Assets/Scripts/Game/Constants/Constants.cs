namespace Game
{
    public static class Constants
    {
        public const float Zero = 0.00001f;
        public const float Speed = 8;
        public const float Rotate = 90;
        public const float MinForce = 10;
        public const float MaxForce = 30;
        public const float MaxChargeTime = 0.75f;
        public const float ChargeSpeed = (MaxForce - MinForce) / MaxChargeTime;
        public const float Hp = 100;
        public const float Attack = 10;
    }
}