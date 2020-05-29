using Base;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class TankUICop : AbsCop
    {
        private float m_TargetForce;
        private float m_TargetHealth;

        private Slider m_ForceSlider;
        private Slider m_HealthSlider;

        public override void OnInit()
        {
            ObjectFeture feture = Entity.GetFeture<ObjectFeture>();
            BaseAttribute attribute = Entity.GetAttribute<BaseAttribute>();

            m_TargetForce = Constants.MinForce;
            m_TargetHealth = attribute.CurHp;

            m_ForceSlider = feture.GetComponent<ViewComponent>().Get<Slider>("1");
            m_ForceSlider.value = m_TargetForce;

            m_HealthSlider = feture.GetComponent<ViewComponent>().Get<Slider>("0");
            m_HealthSlider.minValue = 0;
            m_HealthSlider.maxValue = attribute.MaxHp;
            m_HealthSlider.value = attribute.CurHp;

            RegisterMessage(MessageConstants.Update_Hp, (IBody body) =>
            {
                m_TargetHealth = attribute.CurHp;
                if (m_TargetHealth <= Constants.Zero)
                {
                    m_HealthSlider.value = 0;
                }
            });

            RegisterMessage(MessageConstants.Update_Force, (IBody body) =>
            {
                m_TargetForce = (body as FloatBody).Value;
                if (Mathf.Abs(m_TargetForce - Constants.MinForce) < Constants.Zero)
                {
                    m_ForceSlider.value = m_TargetForce;
                }
            });

            RegisterUpdate(OnUpdate);
        }

        private void OnUpdate(float delta)
        {
            m_ForceSlider.value = Mathf.Lerp(m_ForceSlider.value, m_TargetForce, 0.15f);
            m_HealthSlider.value = Mathf.Lerp(m_HealthSlider.value, m_TargetHealth, 0.15f);
        }
    }
}