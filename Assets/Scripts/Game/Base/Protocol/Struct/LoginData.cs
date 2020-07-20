namespace Zyq.Game.Base.Protocol
{
    public struct LoginTest
    {
        public string Username;
        public string Password;

        public void testWrite(UnityEngine.Networking.NetworkWriter w)
        {
        }

        public void testRead(UnityEngine.Networking.NetworkReader r)
        {
        }
    }

    public struct LoginData
    {
        public string Username;
        public string Password;
        public int[] Scores;
        public LoginTest[] Tests;
        public int K;
        public Login[] Logins;

        public void testWrite(UnityEngine.Networking.NetworkWriter w)
        {
            w.Write(Username);
            w.Write(Password);
            int len1 = Scores != null ? Scores.Length : 0;
            w.Write(len1);
            if (len1 > 0)
            {
                for (int i = 0; i < len1; ++i)
                {
                    w.Write(Scores[i]);
                }
            }
            int len2 = Tests != null ? Tests.Length : 0;
            w.Write(len2);
            if (len2 > 0)
            {
                for (int i = 0; i < len2; ++i)
                {
                    Tests[i].testWrite(w);
                }
            }
        }

        public void testRead(UnityEngine.Networking.NetworkReader r)
        {
            Username = r.ReadString();
            Password = r.ReadString();

            int len1 = r.ReadInt32();
            if (len1 > 0)
            {
                Scores = new int[len1];
                for (int i = 0; i < len1; ++i)
                {
                    Scores[i] = r.ReadInt32();
                }
            }
            int len2 = r.ReadInt32();
            if (len2 > 0)
            {
                Tests = new LoginTest[len2];
                for (int i = 0; i < len1; ++i)
                {
                    Tests[i].testRead(r);
                }
            }
        }
    }
}