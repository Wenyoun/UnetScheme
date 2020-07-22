namespace Zyq.Game.Base.Protocol
{
    public struct LoginTest
    {
        public string Username;
        public string Password;
    }

    public struct LoginData
    {
        public string Username;
        public string Password;
        public int[] Scores;
        public LoginTest[] Tests;
        public Login[] Logins;
    }
}