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
        private Rigidbody _rigidbody;
        public GameObject Target;
        private float _windupTime;

        public bool IsAttacking;

        private List<string> Tags;

        public MeleeAttackAction(BaseEntity entity, GameObject hurtbox, float windup, string tag)
        {
            _entity = entity;
            _hurtbox = hurtbox;
            _windupTime = windup;
            IsAttacking = false;
            _rigidbody = entity.GetComponent<Rigidbody>();
            Tags = new List<string>() { tag };
        }

        public MeleeAttackAction(MeleeAttackActionParameters parameters)
        {
            _entity = parameters.Entity;
            _hurtbox = parameters.HurtBox;
            Target = parameters.Target;
            _windupTime = parameters.WindupTime;
            Tags = parameters.Tags;
        }

        public override void PerformAction()
        {
            if (!IsAttacking && Target != null)
            {
                IsAttacking = true;
                var currentPosition = _rigidbody.position;
                var targetPosition = Target.transform.position;
                var targetVector = (new Vector3(targetPosition.x, 0, targetPosition.z) - new Vector3(currentPosition.x, 0, currentPosition.z));
                _rigidbody.MoveRotation(Quaternion.LookRotation(targetVector));
                Debug.Log("Starting Attack");
                //start windup timer
                _entity.StartCoroutine(WaitForWindup());
                //when completed, start attack.
            }
        }

        public void Cleanup()
        {
            Debug.Log("Ending Attack");
            IsAttacking = false;
            IsCompleted = true;
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
