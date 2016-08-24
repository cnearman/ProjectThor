using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Orc : BaseEnemy, Damagable
    {
        public GameObject Target;
        WalkTowardTargetAction walk;
        Health health;

        public void ApplyDamage(float damage)
        {
            health.Damage(damage);
            if(health.IsDead)
            {
                Die();
            }
        }
        
        [OnAwake]
        public void OrcAwake()
        {
            health = new Health(100.0f);
            walk = new WalkTowardTargetAction(this, 2.0f);
            walk.target = Target.GetComponent<BaseEntity>(); //gameObject.GetComponent<BaseEntity>();
        }

        [OnUpdate]
        public void OrcUpdate()
        {
            walk.PerformAction();
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }
}
