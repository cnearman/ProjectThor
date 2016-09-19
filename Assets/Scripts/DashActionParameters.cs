using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DashActionParameters
    {
        public BaseEntity Entity { get; set; }
        
        public float DashRange { get; set; }

        public float DashDamage { get; set; }

        public Vector3 Target { get; set; }

        public List<string> Tags { get; set; }
    }
}
