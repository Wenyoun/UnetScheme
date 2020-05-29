using Base;

namespace Game
{
    public class GameBattleBehaviour : AbsBehaviour
    {
        public override void OnInit()
        {
            Add<GameBattleModule>();
        }
    }
}