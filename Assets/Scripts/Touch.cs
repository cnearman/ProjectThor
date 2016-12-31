using System;

namespace Assets.Scripts
{
    public class BaseTouch
    {
        public TouchType TouchType = TouchType.Unset;
        public DateTime TouchCompletedUtc = DateTime.UtcNow;
    }
}
