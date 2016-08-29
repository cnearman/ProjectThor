using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class MeleeAttackAction : Action
    {
        private BaseEntity _entity;
        private GameObject _hurtbox;

        private float _windupTime;

        public bool IsAttacking;

        private List<string> Tags;

        public MeleeAttackAction(BaseEntity entity, GameObject hurtbox, float windup, string tag)
        {
            _entity = entity;
            _hurtbox = hurtbox;
            _windupTime = windup;
            IsAttacking = false;
            Tags = new List<string>() { tag };
        }

        public override void PerformAction()
        {
            IsAttacking = true;
            Debug.Log("Starting Attack");
            //start windup timer
            _entity.StartCoroutine(WaitForWindup());
            //when completed, start attack.


        }

        public void Cleanup()
        {
            Debug.Log("Ending Attack");
            IsAttacking = false;
        }

        IEnumerator WaitForWindup()
        {
            Debug.Log("Starting Windup: " + DateTime.Now.Second);
            yield return new WaitForSeconds(_windupTime);
            Debug.Log("Ending Windup: " + DateTime.Now.Second);
            var boxInstance = MonoBehaviour.Instantiate(_hurtbox, _entity.transform.position + (_entity.transform.forward * 2), _entity.transform.rotation);
            var boxRef = ((GameObject)boxInstance).GetComponent<MeleeHurtbox>();
            //when attack has completed set attack to completed.
            boxRef.CleanupMethod = Cleanup;
            boxRef.Tags = Tags;
        }
    }
}
