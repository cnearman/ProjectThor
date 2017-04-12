using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Player : BaseEntity, Damagable
    {
        public bool testingLevel;
        public float abMod;
        public float attackBuffer;
        public float attackSpeed;
        public float attackCooldown;
        public float currentAttackCooldown;

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
        public bool AttackQueued = false;
        private GameObject currentTargetEnemy;
        private float maxAttackDistance = 1.5f;

        public UnityEngine.UI.Text inputType;

        int wallMask = (1 << 12) + (1 << 11);
        Health health;

        NavMeshAgent agent;

        Dictionary<int, Vector2> touchDict = new Dictionary<int, Vector2>();
        public bool isAttacking;
        public GameObject myWeapon;

        Vector3 preTapLocation;

        // Photon Below

        private PhotonView pv;
        private PhotonTransformView ptv;

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

        [OnStart]
        public void PlayerStart()
        {
        }

        [OnAwake]
        public void PlayerAwake()
        {
            attackBuffer = Screen.height / abMod;
            agent = GetComponent<NavMeshAgent>();
            //inputType.text = "FUCK THIS SHIT.";
            _target = target;
            health = new Health(1000.0f);
            tapGrace = Screen.height / 15f;
            pv = GetComponent<PhotonView>();
            ptv = GetComponent<PhotonTransformView>();
            if (pv.isMine)
            {
                TouchManager.RegisterOnTapEventHandler(ProcessTap);
                TouchManager.RegisterOnCircleEventHandler(ProcessCircle);
                TouchManager.RegisterOnLineEventHandler(ProcessLine);
                //TouchManager.RegisterOnArrowUpEventHandler(ProcessArrowUp);
                Camera.main.GetComponent<CameraFollow>().objectTofollow = gameObject;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public void ProcessTap(Tap current)
        {
            preTapLocation = current.Location;

            if (!isAttacking && current.TappedEntity != null)
            {
                AttackQueued = true;
                currentTargetEnemy = current.TappedEntity.gameObject;
            }
            else
            {
                AttackQueued = false;
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
            }
            inputType.text = "TAP";
            Debug.Log("We're doign it guys!");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public void ProcessCircle(CircleGesture current)
        {
            GameObject currentHC = (GameObject)Instantiate(ouchCircle, transform.position, transform.rotation);

            currentHC.transform.position = transform.position;
            currentHC.transform.parent = transform;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        public void ProcessLine(LineGesture current)
        {
            Debug.Log("Line");
            var ray1 = Camera.main.ScreenPointToRay(new Vector3(current.StartingPoint.x, current.StartingPoint.y, 0f));
            var ray2 = Camera.main.ScreenPointToRay(new Vector3(current.EndingPoint.x, current.EndingPoint.y, 0f));

            RaycastHit hit1;
            RaycastHit hit2;
            Vector3 pos1 = new Vector3();
            Vector3 pos2 = new Vector3();
            Debug.DrawRay(ray1.origin, ray1.direction * 10.0f, Color.black, 10);
            if (Physics.Raycast(ray1, out hit1, 100f, ~wallMask))
            {
                // if (hit1.collider.gameObject is typeof();
                // if hits the player then good, we can keep going
                // otherwise we're done here
                if (hit1.collider.CompareTag("Player"))
                {
                    pos1 = hit1.point;
                    if (Physics.Raycast(ray2, out hit2, 100f, wallMask))
                    {
                        pos2 = hit2.point;
                    }
                    if (new Vector3(pos1.x - transform.position.x, 0, pos1.z - transform.position.z).magnitude <= dashStartingGrace)
                    {
                        ClearActions();
                        currentAction = new DashAction(
                            new DashActionParameters
                            {
                                Entity = this,
                                DashDamage = 1,
                                DashRange = 10.0f,
                                Target = new Vector3(pos2.x, transform.position.y, pos2.z),
                                Tags = new List<string> { "Enemy" }
                            });
                    }
                }
            }
            inputType.text = "Line";
        }

       /* public void ProcessArrowUp(ArrowUp current)
        {
            gameObject.GetComponent<Healths>().health++;
        }*/

        [OnUpdate]
        public void PlayerUpdate()
        { 
            if (!pv.isMine && PhotonNetwork.connected)
            {
                return;
            }

            /*
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
                     
            if(Input.GetButtonDown("Fire2"))
            {
                if (!isAttacking && currentAttackCooldown <= 0f)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 100f, wallMask))
                    {
                        
                    }

                    currentAttackCooldown = attackCooldown;
                    isAttacking = true;
                    StartAttack(new Vector3(hit.point.x, 1f, hit.point.z));
                }
            }*/



            if (currentAttackCooldown > 0f)
            {
                currentAttackCooldown -= Time.deltaTime;
            }

            if (currentAction != null && currentAction.IsCompleted) 
            {
                currentAction = nextAction;
                nextAction = thirdAction;
                thirdAction = null;
            }

            if (isAttacking)
            {
                currentAngle += Time.deltaTime * attackSpeed;
                myWeapon.transform.RotateAround(transform.position, transform.up, -Time.deltaTime * attackSpeed);

                if (currentAngle > 180f)
                {
                    isAttacking = false;
                    myWeapon.SetActive(false);
                    agent.ResetPath();
                    agent.Resume();
                    var pos = currentTargetEnemy.transform.position;
                    myWeapon.GetComponentInChildren<PlayerSword>().MakeItSplat(pos);
                    currentTargetEnemy.GetComponent<EHealth>().health--;
                }
            }

            if (AttackQueued)
            {
                if (Vector3.Magnitude(transform.position - currentTargetEnemy.transform.position) < maxAttackDistance)
                {
                    isAttacking = true;
                    currentAttackCooldown = attackCooldown;
                    StartAttack(currentTargetEnemy.transform.position);
                    AttackQueued = false;
                }
                else
                {
                    agent.SetDestination(currentTargetEnemy.transform.position);
                }
            }

            if (currentAction != null)
            {
                Debug.Log("Performing Action :" + currentAction.GetType().Name);
                currentAction.PerformAction();
            }

            ptv.SetSynchronizedValues(GetComponent<Rigidbody>().velocity, 0);
        }
        
        float currentAngle;
        public AudioSource swordSwipe;

        void StartAttack(Vector3 pos)
        {
            agent.Stop();
            transform.LookAt(pos);
            myWeapon.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
            currentAngle = 0f;
            swordSwipe.Play();
            myWeapon.SetActive(true);
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
