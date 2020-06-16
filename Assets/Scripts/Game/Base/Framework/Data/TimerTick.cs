namespace Zyq.Game.Base
{
    public class TimerTick
    {
        public int Id;
        public int Count;
        public float Time;
        public float Interval;
        public System.Action Func;
        public System.Action Frame;
        public System.Action Finish;
        public bool Remove;

        public void Reset()
        {
            Id = 0;
            Count = 0;
            Interval = 0;
            Func = null;
            Frame = null;
            Finish = null;
            Remove = false;
        }
    }
}