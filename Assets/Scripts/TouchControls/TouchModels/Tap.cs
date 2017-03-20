using UnityEngine;

namespace Assets.Scripts
{
    public class Tap : BaseTouch
    {
        public Tap()
        {
            TouchType = TouchType.Tap;
        }

        public Vector3 Location { get; set; }
        public GameObject TappedEntity { get; set; }
    }
}
