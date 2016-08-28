using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeleeHurtbox : BaseEntity
    {
        public System.Action CleanupMethod;
        public float Lifespan = 2.0f;
        private float _damageValue = 40;

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger Enter Melee Hurtbox");
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Effects.Add(new DamageEffect((Damagable)player, _damageValue));
            }
        }

        [OnUpdate]
        public void UpdateHurtboxPosition()
        {
            Debug.Log("Hurtbox Update");
            Destroy(gameObject, Lifespan);
        }

        public void OnDestroy()
        {
            if (CleanupMethod != null)
            {
                Debug.Log("Clean Up");
                CleanupMethod.Invoke();
            }

        }

    }
}
