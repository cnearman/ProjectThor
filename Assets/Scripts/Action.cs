using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class Action
    {
        public bool IsCompleted { get; internal set; }
        public virtual void PerformAction() { }
    }
}
