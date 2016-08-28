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
        TestDamageAction damage;
        WalkTowardTargetAction walk;
        public float grace;

        int wallMask = 1 << 12;
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
            walk = new WalkTowardTargetAction(this, 5.0f);
            walk.target = target.GetComponent<BaseEntity>();
            //damage = new TestDamageAction(Target.GetComponent<BaseEntity>(), 20.0f);
            health = new Health(100.0f);
            damage = new TestDamageAction(target.GetComponent<BaseEntity>(), 20.0f);
        }

        [OnUpdate]
        public void PlayerUpdate()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                damage.PerformAction();
            }

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
            if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), target.transform.position) > grace)
            {
                walk.PerformAction();
            }
        }

        void MoveTo(Ray ray)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, wallMask))
            {
                target.transform.position = hit.point;
                target.GetComponent<Cursor>().PlayPart();
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

    }
}
