namespace Zyq.Game.Base
{
    public class UpdateMgr
    {
        private static UpdateRegister m_Register = new UpdateRegister();

        public static void Init()
        {
            m_Register.OnInit();
        }

        public static void Dispose()
        {
            m_Register.Dispose();
        }

        public static void OnUpdate(float delta)
        {
            m_Register.OnUpdate(delta);
        }

        public static void OnLateUpdate()
        {
            m_Register.OnLateUpdate();
        }

        public static void OnFixedUpdate(float delta)
        {
            m_Register.OnFixedUpdate(delta);
        }

        public static void RegisterUpdate(UpdateDelegate update)
        {
            m_Register.RegisterUpdate(update);
        }

        public static void UnregisterUpdate(UpdateDelegate update)
        {
            m_Register.UnregisterUpdate(update);
        }

        public static void RegisterLateUpdate(LateUpdateDelegate update)
        {
            m_Register.RegisterLateUpdate(update);
        }

        public static void UnregisterLateUpdate(LateUpdateDelegate update)
        {
            m_Register.UnregisterLateUpdate(update);
        }

        public static void RegisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            m_Register.RegisterFixedUpdate(fixedUpdate);
        }

        public static void UnregisterFixedUpdate(UpdateDelegate fixedUpdate)
        {
            m_Register.UnregisterFixedUpdate(fixedUpdate);
        }
    }
}