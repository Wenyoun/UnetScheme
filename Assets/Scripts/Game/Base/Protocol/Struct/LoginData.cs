namespace Zyq.Game.Base.Protocol
{
    public struct LoginData
    {
        public string Username;
        public string Password;
        public int[] Scores;

        public void test(UnityEngine.Networking.NetworkWriter w)
        {
            w.Write(Username);
            w.Write(Password);
            int len1 = Scores.Length;
            w.Write(len1);
            for (int i = 0; i < len1; ++i)
            {
                w.Write(Scores[i]);
            }
        }
    }
}