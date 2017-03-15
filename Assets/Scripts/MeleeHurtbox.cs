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
        private float _damageValue = 2;
        public List<string> Tags;

        [OnAwake]
        public void Initialize()
        {
            if (Tags == null)
            {
                Tags = new List<string>() { string.Empty };
            }
        }

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("Trigger Enter Melee Hurtbox");
            if ( Tags.Any(m => other.CompareTag(m)))
            {
                var entity = other.GetComponent<BaseEntity>();
                if (entity != null)
                {
                    entity.Effects.Add(new DamageEffect((Damagable)entity, _damageValue));
                }
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
