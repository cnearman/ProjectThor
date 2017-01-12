using UnityEngine;

namespace Assets.Scripts
{
    public class LineGesture : BaseTouch
    {

        public LineGesture()
        {
            TouchType = TouchType.Line;
        }

        public Vector3 StartingPoint { get; set; }
        public Vector3 EndingPoint { get; set; }
    }
}
