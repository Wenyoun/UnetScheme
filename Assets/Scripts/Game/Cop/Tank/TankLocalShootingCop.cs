using UnityEngine;

namespace Game
{
    public class TankLocalShootingCop : AbsCop
    {
        private Tick m_Tick;

        private bool m_Fired;
        private float m_CurForce;
        //private TankObject m_Tank;
        private TankShooting m_Shooting;

        public override void OnInit()
        {
            m_Tick = new Tick(0.5f);

            //m_Tank = Entity.GetFeture<ObjectFeture>().GetComponent<TankObject>();
            m_Shooting = Entity.GetFeture<ObjectFeture>().GetComponent<TankShooting>();

            RegisterUpdate(OnUpdate);
        }

        private void OnUpdate(float delta)
        {
            if (!m_Tick.Ready(delta))
            {
                return;
            }

            float force = Constants.MinForce;

            if (Input.GetKeyDown(KeyCode.Space) && !m_Fired)
            {
                m_Fired = true;
                m_CurForce = Constants.MinForce;
            }
            else if (Input.GetKey(KeyCode.Space) && m_Fired)
            {
                m_CurForce += Constants.ChargeSpeed * delta;
                force = m_CurForce;
            }
            else if (Input.GetKeyUp(KeyCode.Space) && m_Fired)
            {
                m_Fired = false;
                Shooting();
            }

            if (m_CurForce >= Constants.MaxForce && m_Fired)
            {
                m_Fired = false;
                m_CurForce = Constants.MaxForce;
                Shooting();
            }

            if(force >= Constants.MaxForce)
            {
                force = Constants.MinForce;
            }

            m_Shooting.CmdForce(force);
            EntityMgr.Dispatcher(MessageConstants.Update_Force, Entity.Eid, FloatBody.Default.Init(force));
        }

        private void Shooting()
        {
            m_Tick.Reset();
            m_Shooting.CmdShooting(m_CurForce);
        }
    }
}