using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity, Damagable
    {
        public GameObject target;
        private GameObject _target;
        //TestDamageAction damage;
        WalkTowardTargetAction walk;
        MeleeAttackAction attack;
        public float grace;
        public float attackRange;
        public GameObject hurtbox;

        public Action currentAction;
        public Action nextAction;
        public Action thirdAction;

        int wallMask = (1 << 12) + (1 << 11);
        Health health;

        private bool _performingAction
        {
            get
            {
                return currentAction != null && (!currentAction.IsCompleted || nextAction != null);
            }
        }

        private bool _performingAttack
        {
            get
            {
                return currentAction != null && !currentAction.IsCompleted && currentAction is MeleeAttackAction;
            }
        }

        private bool _performingMove
        {
            get
            {
                return currentAction != null && !currentAction.IsCompleted && currentAction is WalkTowardTargetAction;
            }
        }

        public void ApplyDamage(float damage)
        {
            health.Damage(damage);
            if (health.IsDead)
            {
                Die();
            }
        }

        [OnAwake]
        public void PlayerAwake()
        {
            _target = target;
            walk = new WalkTowardTargetAction(this, 5.0f);
            attack = new MeleeAttackAction(this, hurtbox, 0.0f, "Enemy");
            Retarget();
            health = new Health(1000.0f);
        }

        [OnUpdate]
        public void PlayerUpdate()
        {
            //If touch doesn't touch an enemy
            //And top of Queue is move
            // Clear Queue and Queue up move
            //If touch does touch an enemy can can attack it
            // Queue up attack
            //If touch does touch an enemy and can't attack it
            // queue up move and attack.

            if (currentAction != null && currentAction.IsCompleted) 
            {
                currentAction = nextAction;
                nextAction = thirdAction;
                thirdAction = null;
            }

            foreach (Touch currentTouches in Input.touches)
            {
                if (currentTouches.phase == TouchPhase.Ended)
                {
                    var ray = Camera.main.ScreenPointToRay(currentTouches.position);
                    ProcessInput(ray);
                }
            }

            if (currentAction != null)
            {
                Debug.Log("Performing Action :" + currentAction.GetType().Name);
                currentAction.PerformAction();
            }
        }

        void ProcessInput(Ray ray)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, wallMask))
            {
                var enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();

                if ( enemy != null) // Touch was an enemy
                {
                    _target = enemy.gameObject;
                    var pPos = gameObject.transform.position;
                    var ePos = _target.transform.position;
                    if (Vector3.Distance(new Vector3(pPos.x, 0, pPos.z), new Vector3(ePos.x, 0, ePos.z)) <= attackRange) // within range
                    {
                        if (_performingAttack)
                        {
                            Debug.Log("Queue Second Attack");
                            attack.Target = _target;
                            nextAction = attack;
                        }
                        else
                        {
                            Debug.Log("Queue First Attack");
                            ClearActions();
                            attack.Target = _target;
                            currentAction = attack;
                        }
                    }
                    else
                    {
                        if (_performingAttack)
                        {
                            Debug.Log("Queue Second Walk Then Attack");
                            nextAction = walk;
                            attack.Target = _target;
                            thirdAction = attack;
                        }
                        else
                        {
                            Debug.Log("Queue Walk Then Attack");
                            currentAction = walk;
                            attack.Target = _target;
                            nextAction = attack;
                        }

                    }
                }
                else // Touch was not an enemy
                {
                    if (_performingAttack)
                    {
                        Debug.Log("Queue wait then, New Walk to point");
                        nextAction = walk;
                        _target = target;
                        _target.transform.position = hit.point;
                        _target.GetComponent<Cursor>().PlayPart();
                    }
                    else
                    {
                        Debug.Log("Queue New Walk to point");
                        ClearActions();
                        currentAction = walk;
                        _target = target;
                        _target.transform.position = hit.point;
                        _target.GetComponent<Cursor>().PlayPart();
                    }
                }
                Retarget();
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void Retarget()
        {
            var newTarget = _target.GetComponent<BaseEntity>();
            walk.target = newTarget;
        }

        void ClearActions()
        {
            currentAction = null;
            nextAction = null;
        }

    }
}
