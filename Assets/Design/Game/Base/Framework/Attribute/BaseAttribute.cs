namespace Zyq.Game.Base
{
    public class BaseAttribute : IAttribute
    {
        public float CurHp;
        public float MaxHp;

        public BaseAttribute(float hp)
        {
            CurHp = hp;
            MaxHp = hp;
        }

        public void Hurt(float damage)
        {
            CurHp -= damage;
            if (CurHp < 0)
            {
                CurHp = 0;
            }
        }
    }
}