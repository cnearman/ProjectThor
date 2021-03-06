﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Player : BaseEntity, Damagable
    {
        List<Point> currentPoints = new List<Point>();
        Point firstPoint;
        Gesture[] trainers;

        public GameObject ouchCircle;

        float tapGrace;

        public GameObject target;
        private GameObject _target;
        WalkTowardTargetAction walk;
        MeleeAttackAction attack;
        public float grace;
        public float attackRange;
        public GameObject hurtbox;

        public Action currentAction;
        public Action nextAction;
        public Action thirdAction;

        public float dashStartingGrace = 2.0f;

        int wallMask = (1 << 12) + (1 << 11);
        Health health;

        UnityEngine.AI.NavMeshAgent agent;

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
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            _target = target;
            //walk = new WalkTowardTargetAction(this, 5.0f);
            //attack = new MeleeAttackAction(this, hurtbox, 0.0f, "Enemy");
            health = new Health(1000.0f);

            tapGrace = Screen.height / 15f;

            List<Point> circlePoints = new List<Point>();

            for (float i = 0; i < 360f; i += 11.25f)
            {
                float x = 10f * Mathf.Cos(i);
                float y = 10f * Mathf.Sin(i);

                Point tempC = new Point(x, y, 0);

                circlePoints.Add(tempC);
            }

            Gesture rCircle = new Gesture(circlePoints.ToArray(), "aCircle");

            trainers = new Gesture[] { rCircle };
        }

        [OnUpdate]
        public void PlayerUpdate()
        {
            //for testing
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f, wallMask))
                {
                    preTapLocation = hit.point;
                    PostTapAction();
                }

            }

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
                if (currentTouches.phase == TouchPhase.Began)
                {
                    currentPoints.Clear();
                    Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                    currentPoints.Add(cPoint);
                    firstPoint = cPoint;

                    var ray = Camera.main.ScreenPointToRay(currentTouches.position);
                    PreTapAction(ray);
                }
                else if (currentTouches.phase == TouchPhase.Ended)
                {

                    Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                    currentPoints.Add(cPoint);

                    bool isTap = true;

                    //do tap test
                    Point lastPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                    foreach (Point p in currentPoints)
                    {
                        if (Geometry.EuclideanDistance(p, lastPoint) > tapGrace)
                        {
                            Debug.Log("Not a tap");
                            isTap = false;
                            break;
                        }
                    }

                    if (isTap)
                    {
                        //var rayT = Camera.main.ScreenPointToRay(new Vector3(lastPoint.X, lastPoint.Y, 0f));

                        PostTapAction();
                    }
                    else
                    {
                        Point[] tempLine = { firstPoint, lastPoint };
                        Gesture lineGesture = new Gesture(tempLine, "tLine");

                        Gesture[] tempTrainers = { trainers[0], lineGesture };

                        //send to PDollar
                        Point[] pointArray = currentPoints.ToArray();
                        Gesture myGesture = new Gesture(pointArray);
                        string nameOfShape = PointCloudRecognizer.Classify(myGesture, tempTrainers);

                        if (nameOfShape == "aCircle")
                        {

                            GameObject currentHC = (GameObject)Instantiate(ouchCircle, transform.position, transform.rotation);

                            currentHC.transform.position = transform.position;
                            currentHC.transform.parent = transform;
                        }
                        else if(nameOfShape == "tLine")
                        {
                            Debug.Log("Line");
                            var ray1 = Camera.main.ScreenPointToRay(new Vector3(firstPoint.X, firstPoint.Y, 0f));
                            var ray2 = Camera.main.ScreenPointToRay(new Vector3(lastPoint.X, lastPoint.Y, 0f));

                            RaycastHit hit1;
                            RaycastHit hit2;
                            Vector3 pos1 = new Vector3();
                            Vector3 pos2 = new Vector3();
                            if (Physics.Raycast(ray1, out hit1, 100f, wallMask))
                            {
                                pos1 = hit1.point;
                            }

                            if (Physics.Raycast(ray2, out hit2, 100f, wallMask))
                            {
                                pos2 = hit2.point;
                            }
                            if (new Vector3(pos1.x - transform.position.x, 0 , pos1.z - transform.position.z).magnitude <= dashStartingGrace)
                            {
                                ClearActions();
                                currentAction = new DashAction(
                                    new DashActionParameters
                                    {
                                        Entity = this,
                                        DashDamage = 30,
                                        DashRange = 10.0f,
                                        Target = new Vector3(pos2.x, transform.position.y, pos2.z),
                                        Tags = new List<string> { "Enemy" }
                                    });
                            }
                        }
                    }
                }
                else
                {
                    Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                    currentPoints.Add(cPoint);
                }
            }

            if (currentAction != null)
            {
                Debug.Log("Performing Action :" + currentAction.GetType().Name);
                currentAction.PerformAction();
            }
        }

        void HurtCircle(Ray origin, Ray rad)
        {
            RaycastHit hit1;
            RaycastHit hit2;

            float fX = 0f;
            float dX;

            GameObject currentHC = (GameObject) Instantiate(ouchCircle, transform.position, transform.rotation);
            //Destroy(currentHC, 1f);

            if (Physics.Raycast(origin, out hit1, 100f, wallMask))
            {
                currentHC.transform.position = hit1.point;
                fX = hit1.point.x;
            }

            if (Physics.Raycast(rad, out hit2, 100f, wallMask))
            {
                dX = Mathf.Abs(fX - hit2.point.x) * 2f;
                currentHC.transform.localScale = new Vector3(dX, 0.3f, dX);

            }
        }

        Vector3 preTapLocation;
        GameObject preTapEnemy;
        bool preTapHit;

        void PreTapAction(Ray preTap)
        {
            preTapHit = false;
            preTapLocation = Vector3.zero;
            preTapEnemy = null;
            RaycastHit hit;

            if (Physics.Raycast(preTap, out hit, 100f, wallMask))
            {
                preTapLocation = hit.point;

                var enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    preTapEnemy = enemy.gameObject;
                    preTapHit = true;
                }
            }
        }

        void PostTapAction()
        {
            if (preTapEnemy != null) // Touch was an enemy
            {
                _target = preTapEnemy.gameObject;
                var pPos = gameObject.transform.position;
                var ePos = _target.transform.position;
                if (Vector3.Distance(new Vector3(pPos.x, 0, pPos.z), new Vector3(ePos.x, 0, ePos.z)) <= attackRange) // within range
                {
                    if (_performingAttack)
                    {
                        Debug.Log("Queue Second Attack");
                        //TODO: Stop multiple queueing of melee attack actions.
                        // This might be fine, but it's not a very robust system.
                        nextAction = new MeleeAttackAction(new MeleeAttackActionParameters
                        {
                            Entity = this,
                            Target = _target,
                            HurtBox = hurtbox,
                            WindupTime = 0.0f,
                            Tags = new List<string> { "Enemy" }
                        });
                    }
                    else
                    {
                        Debug.Log("Queue First Attack");
                        ClearActions();
                        currentAction = new MeleeAttackAction(new MeleeAttackActionParameters
                        {
                            Entity = this,
                            Target = _target,
                            HurtBox = hurtbox,
                            WindupTime = 0.0f,
                            Tags = new List<string> { "Enemy" }
                        });
                    }
                }
                else
                {
                    if (_performingAttack)
                    {
                        Debug.Log("Queue Second Walk Then Attack");
                        nextAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                        {
                            Entity = this,
                            Target = _target.GetComponent<BaseEntity>(),
                            MovementSpeed = 5.0f,
                            GraceRange = 0.3f
                        });
                        thirdAction = new MeleeAttackAction(new MeleeAttackActionParameters
                        {
                            Entity = this,
                            Target = _target,
                            HurtBox = hurtbox,
                            WindupTime = 0.0f,
                            Tags = new List<string> { "Enemy" }
                        });
                    }
                    else
                    {
                        Debug.Log("Queue Walk Then Attack");
                        currentAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                        {
                            Entity = this,
                            Target = _target.GetComponent<BaseEntity>(),
                            MovementSpeed = 5.0f,
                            GraceRange = 0.3f
                        });
                        nextAction = new MeleeAttackAction(new MeleeAttackActionParameters
                        {
                            Entity = this,
                            Target = _target,
                            HurtBox = hurtbox,
                            WindupTime = 0.0f,
                            Tags = new List<string> { "Enemy" }
                        });
                    }

                }
            }
            else // Touch was not an enemy
            {
                
                //target.transform.position = preTapLocation;
                //target.GetComponent<Cursor>().PlayPart();
                //agent.SetDestination(target.transform.position);

                if (_performingAttack)
                {
                    Debug.Log("Queue wait then, New Walk to point");

                    _target = target;
                    _target.transform.position = preTapLocation;
                    _target.GetComponent<Cursor>().PlayPart();

                    nextAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                    {
                        Entity = this,
                        Target = target.GetComponent<BaseEntity>(),
                        MovementSpeed = 5.0f,
                        GraceRange = 0.3f
                    });
                }
                else
                {
                    Debug.Log("Queue New Walk to point");
                    ClearActions();

                    _target = target;
                    _target.transform.position = preTapLocation;
                    _target.GetComponent<Cursor>().PlayPart();

                    currentAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                    {
                        Entity = this,
                        Target = target.GetComponent<BaseEntity>(),
                        MovementSpeed = 5.0f,
                        GraceRange = 0.3f
                    });
                }


                /*if (_performingAttack)
                {
                    Debug.Log("Queue wait then, New Walk to point");
                    nextAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                    {
                        Entity = this,
                        Target = target.GetComponent<BaseEntity>(),
                        MovementSpeed = 5.0f,
                        GraceRange = 0.3f
                    });
                    _target = target;
                    _target.transform.position = preTapLocation;
                    _target.GetComponent<Cursor>().PlayPart();
                }
                else
                {
                    Debug.Log("Queue New Walk to point");
                    ClearActions();
                    currentAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                    {
                        Entity = this,
                        Target = target.GetComponent<BaseEntity>(),
                        MovementSpeed = 5.0f,
                        GraceRange = 0.3f
                    });
                    _target = target;
                    _target.transform.position = preTapLocation;
                    _target.GetComponent<Cursor>().PlayPart();
                }*/
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }

        void ClearActions()
        {
            currentAction = null;
            nextAction = null;
        }

    }
}
