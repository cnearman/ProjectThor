using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class TestDamageAction : Action
    {
        BaseEntity _target;
        float _damageValue;

        public TestDamageAction(BaseEntity target, float damageValue)
        {
            _target = target;
            _damageValue = damageValue;
        }

        public override void PerformAction()
        {
            if (_target is Damagable)
            {
                _target.Effects.Add(new DamageEffect((Damagable)_target, _damageValue));
            }
        }
    }
}
