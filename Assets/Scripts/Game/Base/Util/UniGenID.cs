namespace Zyq.Game.Base
{
    public static class UniGenID
    {
        private static uint Start_Cop_Id = 1;
        private static uint Start_Entity_Id = 1;

        public static uint GenNextCopID()
        {
            return Start_Cop_Id++;
        }

        public static uint GenNextEntityID()
        {
            return Start_Entity_Id++;
        }
    }
}