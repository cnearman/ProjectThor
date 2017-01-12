using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    public class CircleGesture : BaseTouch
    {
        public CircleGesture()
        {
            TouchType = TouchType.Circle;
        }
    }
}
