using UnityEngine;

namespace Assets.Scripts
{
    public class Orc : BaseEnemy
    {
        public GameObject Target;
        WalkTowardTargetAction walk;

        //On Update, move toward target.
        void Awake()
        {
            walk = new WalkTowardTargetAction(this, 2.0f);
            walk.target = Target.GetComponent<BaseEntity>(); //gameObject.GetComponent<BaseEntity>();
        }

        void FixedUpdate()
        {
            walk.PerformAction();
        }
    }
}
