namespace Zyq.Game.Proto
{
    public struct LoginData
    {
        public string Username;
        public string Password;
        public int[] Scores;
        public Login[] Logins;
        public int Final;
    }
}