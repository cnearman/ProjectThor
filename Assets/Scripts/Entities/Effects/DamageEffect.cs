using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class DamageEffect : Effect
    {
        private IDamagable _damageTarget;
        private float _damageAmount;

        public DamageEffect(IDamagable damageTarget, float damageAmount)
        {
            _damageTarget = damageTarget;
            _damageAmount = damageAmount;
        }

        public override void ApplyEffect()
        {
            _damageTarget.ApplyDamage(_damageAmount);
            IsEffectComplete = true;
        }
    }
}
