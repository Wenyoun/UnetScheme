namespace Nice.Game.Base
{
    public static class UniGenID
    {
        private static uint StartCopId = 1;
        private static uint StartEntityId = 1;

        public static uint GenNextCopID()
        {
            return StartCopId++;
        }

        public static uint GenNextEntityID()
        {
            return StartEntityId++;
        }
    }
}