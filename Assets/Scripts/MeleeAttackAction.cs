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

        public bool IsAttacking;

        public MeleeAttackAction(BaseEntity entity, GameObject hurtbox)
        {
            _entity = entity;
            _hurtbox = hurtbox;
            IsAttacking = false;
        }

        public override void PerformAction()
        {
            IsAttacking = true;
            Debug.Log("Starting Attack");
            //start windup timer
            _entity.StartCoroutine(WaitForWindup());
            //when completed, start attack.
            Debug.Log("Ending Windup");
            var boxInstance = MonoBehaviour.Instantiate(_hurtbox, _entity.transform.position, _entity.transform.rotation );
            var boxRef = ((GameObject)boxInstance).GetComponent<MeleeHurtbox>();
            //when attack has completed set attack to completed.
            boxRef.CleanupMethod = Cleanup;
        }

        public void Cleanup()
        {
            Debug.Log("Ending Attack");
            IsAttacking = false;
        }

        IEnumerator WaitForWindup()
        {
            Debug.Log("Starting Windup");
            yield return new WaitForSeconds(0.4f);
        }
    }
}
