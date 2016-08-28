using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity, Damagable
    {
        public GameObject Target;
        TestDamageAction damage;
        Health health;

        public void ApplyDamage(float damage)
        {
            health.Damage(damage);
            if (health.IsDead)
            {
                Die();
            }
        }

        public override void Awake()
        {
            base.Awake();
            health = new Health(100.0f);
            damage = new TestDamageAction(Target.GetComponent<BaseEntity>(), 20.0f);
        }

        public override void Update()
        {
            base.Update();
            if(Input.GetButtonDown("Fire1"))
            {
                damage.PerformAction();
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

    }
}
