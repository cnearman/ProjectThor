using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Orc : BaseEnemy, Damagable
    {
        public GameObject Target;
        public GameObject Hurtbox;
        WalkTowardTargetAction walk;
        MeleeAttackAction attack;
        Health health;
        public float walkRange = 2.5f;
        public float attackRange = 2.5f;

        private Animator _animator;

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
            walk.target = Target.GetComponent<BaseEntity>();
            attack = new MeleeAttackAction(this, Hurtbox);
            _animator = GetComponent<Animator>();
        }

        [OnUpdate]
        public void OrcUpdate()
        {
            //if within attack range, Attack, 
            //else walk.
            if (_shouldAttack)
            {
                attack.PerformAction();
            }

            _animator.SetBool("Attack", attack.IsAttacking);

            if (_shouldWalk)
            {
                walk.PerformAction();
                _animator.SetBool("Walk", true);
            }
            else
            {
                _animator.SetBool("Walk", false);
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        private bool _shouldWalk
        {
            get
            {
                var distance = Target.transform.position - transform.position;
                distance.y = 0;
                return distance.magnitude > walkRange && !_shouldAttack;
            }
        }

        private bool _shouldAttack
        {
            get
            {
                var distance = Target.transform.position - transform.position;
                distance.y = 0;
                return distance.magnitude <= attackRange && !attack.IsAttacking;
            }
        }
    }
}
