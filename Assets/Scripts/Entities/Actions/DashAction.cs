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
        private UnityEngine.AI.NavMeshAgent agent;
        public List<string> Tags = new List<string> { };

        public DashAction(DashActionParameters parameters)
        {
            _rigidbody = parameters.Entity.GetComponent<Rigidbody>();
            agent = parameters.Entity.GetComponent<UnityEngine.AI.NavMeshAgent>();
            DashRange = parameters.DashRange;
            DashDamage = parameters.DashDamage;
            Target = parameters.Target;
            Tags = parameters.Tags;
        }

        public override void PerformAction()
        {
            IsCompleted = false;
            agent.ResetPath();
            agent.isStopped = true;
            Debug.Log("Dash");

            var startingPosition = _rigidbody.transform.position;
            agent.nextPosition = Target;
            var endingPosition = _rigidbody.transform.position;
            var dir = endingPosition - startingPosition;
            var hits = Physics.RaycastAll(startingPosition, dir, DashRange);

            foreach (var hit in hits)
            {
                var other = hit.collider;
                if (Tags.Any(m => other.CompareTag(m)))
                {
                    var entity = other.GetComponent<BaseEntity>();
                    if (entity != null)
                    {
                        entity.Effects.Add(new DamageEffect((IDamagable)entity, DashDamage));
                    }
                }
                // TODO: Handle obstacles
            }
            IsCompleted = true;
            agent.isStopped = false;
        }
    }
}
