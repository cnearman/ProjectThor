using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class DashAction : Action
    {
        private Rigidbody _rigidbody;

        private float DashRange = 0.0f;
        private float DashDamage = 0.0f;

        private Vector3 Target = Vector3.zero;
        private NavMeshAgent agent;
        public List<string> Tags = new List<string> { };

        public DashAction(DashActionParameters parameters)
        {
            _rigidbody = parameters.Entity.GetComponent<Rigidbody>();
            agent = parameters.Entity.GetComponent<NavMeshAgent>();
            DashRange = parameters.DashRange;
            DashDamage = parameters.DashDamage;
            Target = parameters.Target;
            Tags = parameters.Tags;
        }

        public override void PerformAction()
        {
            IsCompleted = false;
            agent.ResetPath();
            Debug.Log("Dash");
            var hits = Physics.RaycastAll(_rigidbody.transform.position, Target - _rigidbody.transform.position, DashRange);
            foreach ( var hit in hits)
            {
                var other = hit.collider;
                if(Tags.Any(m => other.CompareTag(m)))
                {
                    var entity = other.GetComponent<BaseEntity>();
                    if (entity != null)
                    {
                        entity.Effects.Add(new DamageEffect((Damagable)entity, DashDamage));
                    }
                }
                // TODO: Handle obstacles
            }

            _rigidbody.MovePosition(Target);

            IsCompleted = true;
            agent.Resume();
        }
    }
}
