using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeleeAttackActionParameters
    {
        public BaseEntity Entity { get; set; }

        public GameObject Target { get; set; }

        public GameObject HurtBox { get; set; }

        public float WindupTime { get; set; }

        public List<string> Tags { get; set; }
    }
}
