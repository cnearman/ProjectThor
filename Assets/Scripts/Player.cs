using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity
    {
        public GameObject target;
        TestDamageAction damage;
        WalkTowardTargetAction walk;
        public float grace;

        int wallMask = 1 << 12;

        public override void Awake()
        {
            base.Awake();
            walk = new WalkTowardTargetAction(this, 5.0f);
            walk.target = target.GetComponent<BaseEntity>();
            //damage = new TestDamageAction(Target.GetComponent<BaseEntity>(), 20.0f);
        }


        public override void Update()
        {
            base.Update();
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

        void FixedUpdate()
        {
            if (Vector3.Distance(gameObject.transform.position - new Vector3(0f,1f,0f), target.transform.position) > grace)
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
    }
}
