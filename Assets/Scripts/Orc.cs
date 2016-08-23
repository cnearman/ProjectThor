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

        //On Update, move toward target.
        public override void Awake()
        {
            base.Awake();
            health = new Health(100.0f);
            walk = new WalkTowardTargetAction(this, 2.0f);
            walk.target = Target.GetComponent<BaseEntity>(); //gameObject.GetComponent<BaseEntity>();
        }

        void FixedUpdate()
        {
            walk.PerformAction();
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }
}
