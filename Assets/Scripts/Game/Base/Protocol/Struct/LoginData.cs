namespace Zyq.Game.Base.Protocol
{
    public struct LoginData
    {
        public string Username;
        public string Password;
        public int[] Scores;
        public Login[] Logins;
    }
}