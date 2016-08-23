using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity
    {
        public GameObject Target;
        TestDamageAction damage;

        public override void Awake()
        {
            base.Awake();
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
    }
}
