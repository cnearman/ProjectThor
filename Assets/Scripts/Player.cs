using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity, Damagable
    {
        public GameObject target;
        private GameObject _target;
        //TestDamageAction damage;
        WalkTowardTargetAction walk;
        MeleeAttackAction attack;
        public float grace;
        public float attackRange;
        public GameObject hurtbox;

        int wallMask = (1 << 12) + (1 << 11);
        Health health;

        public void ApplyDamage(float damage)
        {
            health.Damage(damage);
            if (health.IsDead)
            {
                Die();
            }
        }

        [OnAwake]
        public void PlayerAwake()
        {
            _target = target;
            walk = new WalkTowardTargetAction(this, 5.0f);
            attack = new MeleeAttackAction(this, hurtbox, 0.0f, "Enemy");
            Retarget();
            //damage = new TestDamageAction(Target.GetComponent<BaseEntity>(), 20.0f);
            health = new Health(1000.0f);
            //damage = new TestDamageAction(_target.GetComponent<BaseEntity>(), 20.0f);
        }

        [OnUpdate]
        public void PlayerUpdate()
        {
            /*if (Input.GetButtonDown("Fire1"))
            {
                damage.PerformAction();
            }*/

            foreach (Touch currentTouches in Input.touches)
            {
                if (currentTouches.phase == TouchPhase.Ended)
                {
                    var ray = Camera.main.ScreenPointToRay(currentTouches.position);
                    MoveTo(ray);
                }
            }
        }

        [OnFixedUpdate]
        public void PlayerFixedUpdate()
        {
            Debug.Log("Fixed Update");
            if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), _target.transform.position) > grace)
            {
                walk.PerformAction();
            }
        }

        void MoveTo(Ray ray)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, wallMask))
            {
                var enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();
                if ( enemy != null)
                {
                    _target = enemy.gameObject;
                    if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), _target.transform.position) <= attackRange)
                    {
                        attack.PerformAction();
                        _target = target;
                        _target.transform.position = hit.point;
                        _target.GetComponent<Cursor>().PlayPart();
                    }
                }
                else
                {
                    _target = target;
                    _target.transform.position = hit.point;
                    _target.GetComponent<Cursor>().PlayPart();
                }
                Retarget();
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void Retarget()
        {
            var newTarget = _target.GetComponent<BaseEntity>();
            walk.target = newTarget;
        }

    }
}
