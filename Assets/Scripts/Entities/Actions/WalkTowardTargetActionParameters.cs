using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class WalkTowardTargetActionParameters
    {
        public BaseEntity Entity { get; set; }

        public BaseEntity Target { get; set; }
        
        public float MovementSpeed { get; set; }
        
        public float GraceRange { get; set; }
    }
}
