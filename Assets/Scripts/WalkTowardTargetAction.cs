﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class WalkTowardTargetAction : Action
    {
        private Rigidbody _rigidbody;
        private float MovementSpeed;

        private float GraceRange = 1.0f;

        public BaseEntity target { get; set; }

        public WalkTowardTargetAction(BaseEntity entity, float ms)
        {
            _rigidbody = entity.GetComponent<Rigidbody>();
            MovementSpeed = ms;
        }

        public override void PerformAction()
        {
            IsCompleted = false;
            var currentPosition = _rigidbody.position;
            var targetPosition = target.transform.position;
            var targetVector = (new Vector3(targetPosition.x, 0, targetPosition.z) - new Vector3(currentPosition.x, 0, currentPosition.z));
            if (targetVector.magnitude > GraceRange)
            {
                targetVector.Normalize();
                _rigidbody.MovePosition(currentPosition + (targetVector * MovementSpeed) * Time.deltaTime);
                _rigidbody.MoveRotation(Quaternion.LookRotation(targetVector));
            }
            else
            {
                IsCompleted = true;
            }
        }
    }
}
