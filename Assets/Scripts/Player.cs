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
        //TestDamageAction damage;
        WalkTowardTargetAction walk;
        MeleeAttackAction attack;
        public float grace;
        public float attackRange;
        public GameObject hurtbox;

        int wallMask = (1 << 12) + (1 << 11);
        Health health;

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
            //damage = new TestDamageAction(Target.GetComponent<BaseEntity>(), 20.0f);
            health = new Health(1000.0f);
            //damage = new TestDamageAction(_target.GetComponent<BaseEntity>(), 20.0f);

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
            /*if (Input.GetButtonDown("Fire1"))
            {
                damage.PerformAction();
            }*/

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
                            /*float totalX = 0f;
                            float totalY = 0f;

                            float aveX = 0f;
                            float aveY = 0f;

                            float minY = pointArray[0].Y;
                            float maxY = pointArray[0].Y;

                            float minX = pointArray[0].X;
                            float maxX = pointArray[0].X;

                            for (int j = 0; j < pointArray.Length; j++)
                            {
                                totalX += pointArray[j].X;
                                totalY += pointArray[j].Y;

                                if (pointArray[j].X < minX)
                                {
                                    minX = pointArray[j].X;
                                }
                                if (pointArray[j].X > maxX)
                                {
                                    maxX = pointArray[j].X;
                                }
                                if (pointArray[j].Y < minY)
                                {
                                    minY = pointArray[j].Y;
                                }
                                if (pointArray[j].Y > maxY)
                                {
                                    maxY = pointArray[j].Y;
                                }

                            }

                            float myRadius = ((Mathf.Abs((maxX - minX)) + Mathf.Abs((maxY - minY))) / 2f) / 2f;

                            aveX = totalX / pointArray.Length;
                            aveY = totalY / pointArray.Length;

                            Debug.Log(minX + ", " + maxX);

                            var ray3 = Camera.main.ScreenPointToRay(new Vector3(aveX, aveY, 0f));
                            var ray4 = Camera.main.ScreenPointToRay(new Vector3(aveX + myRadius, aveY, 0f));

                            HurtCircle(ray3, ray4);*/
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

        [OnFixedUpdate]
        public void PlayerFixedUpdate()
        {
            Debug.Log("Fixed Update");
            if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), _target.transform.position) > grace)
            {
                walk.PerformAction();
            }
        }

        void MoveTo(Ray ray)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, wallMask))
            {
                var enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();
                if ( enemy != null)
                {
                    _target = enemy.gameObject;
                    if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), _target.transform.position) <= attackRange)
                    {
                        attack.PerformAction();
                        _target = target;
                        _target.transform.position = hit.point;
                        _target.GetComponent<Cursor>().PlayPart();
                    }
                }
                else
                {
                    _target = target;
                    _target.transform.position = hit.point;
                    _target.GetComponent<Cursor>().PlayPart();
                }
                Retarget();
            }
        }

        Vector3 preTapLocation;
        GameObject preTapEnemy;
        bool preTapHit;

        void PreTapAction(Ray preTap)
        {
            preTapHit = false;

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
            if(preTapHit && preTapEnemy != null)
            {
                _target = preTapEnemy.gameObject;
                if (Vector3.Distance(gameObject.transform.position - new Vector3(0f, 1f, 0f), _target.transform.position) <= attackRange)
                {
                    attack.PerformAction();
                    _target = target;
                    _target.transform.position = transform.position;
                    _target.GetComponent<Cursor>().PlayPart();
                }
            } else
            {
                _target = target;
                _target.transform.position = preTapLocation;
                _target.GetComponent<Cursor>().PlayPart();
            }

            Retarget();
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

    }
}
