using System;
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
                        Gesture[] tempTrainers = { trainers[0] };

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
                    }

                    //var ray = Camera.main.ScreenPointToRay(currentTouches.position);
                    //MoveTo(ray);
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
                            GraceRange = 2.5f
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
                            GraceRange = 2.5f
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
                if (_performingAttack)
                {
                    Debug.Log("Queue wait then, New Walk to point");
                    nextAction = new WalkTowardTargetAction(new WalkTowardTargetActionParameters
                    {
                        Entity = this,
                        Target = target.GetComponent<BaseEntity>(),
                        MovementSpeed = 5.0f,
                        GraceRange = 2.5f
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
                        GraceRange = 2.5f
                    });
                    _target = target;
                    _target.transform.position = preTapLocation;
                    _target.GetComponent<Cursor>().PlayPart();
                }
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
