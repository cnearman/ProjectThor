using UnityEngine;
using System.Collections;

public class UrukHai : BaseEnemyMob {
    public float moveSpeedPatrol;
    public float moveSpeedHighAlert;
    public float moveSpeedMoveToAttack;
    public float moveSpeedPrepareNextAttack;

    public AudioSource swiper;

    public float patrolRange;
    float startX;
    float startZ;

    public float patrolUpdate;
    float currentPatrolUpdate;
    
    public GameObject weapon;

    public float attackSpeed;
    public float pullBackSpeed;
    
    UnityEngine.AI.NavMeshAgent agent;

    public GameObject hurtBox;
    public float hurtTime;

    public bool attacking;
    public bool coolDown;

    // Use this for initialization
    protected override void Start () {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        startX = transform.position.x;
        startZ = transform.position.z;

        currentAngle = 0;

        currentState = possibleState.Patrol;
        currentTarget = GameObject.Find("Player");
    }

    protected override void Update()
    {
        if (currentState == possibleState.Patrol)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            currentTarget = null;
            float distToPlayer = Mathf.Infinity;
            foreach(GameObject currentPlayer in players)
            {
                float dist = Vector3.Distance(transform.position, currentPlayer.transform.position);
                if(dist < distToPlayer)
                {
                    distToPlayer = dist;
                    currentTarget = currentPlayer;
                }
            }
            
        }
        base.Update();
    }

    protected override void Patrol()
    {
        agent.speed = moveSpeedPatrol;
        if (currentPatrolUpdate < 0f)
        {
            PickPatrolPosition();

            currentPatrolUpdate = patrolUpdate;
        }
        else
        {
            currentPatrolUpdate -= Time.deltaTime;
        }
    }

    protected override void MoveToAttack()
    {
        agent.speed = moveSpeedMoveToAttack;
        agent.SetDestination(currentTarget.transform.position);
    }

    protected override void PoweringUp()
    {
        agent.Stop();
        transform.LookAt(currentTarget.transform);
    }

    protected override void StartAttack()
    {
        attacking = true;
        swiper.Play();
    }

    protected override void Attack()
    {
        if (coolDown)
        {
            PullBackWeapon();
        }
        else if (attacking)
        {
            AttackStuff();
        }
    }

    protected override void PrepareNextAttack()
    {
        agent.speed = moveSpeedPrepareNextAttack;
        agent.SetDestination(currentTarget.transform.position);
    }

    protected override void HighAlertAction()
    {
        agent.speed = moveSpeedHighAlert;

        if (currentPatrolUpdate < 0f)
        {
            PickPatrolPosition();

            currentPatrolUpdate = patrolUpdate;
        }
        else
        {
            currentPatrolUpdate -= Time.deltaTime;
        }
    }
    

    void PickPatrolPosition()
    {
        float pickX = Random.Range(-patrolRange, patrolRange);
        float pickZ = Random.Range(-patrolRange, patrolRange);

        pickX += startX;
        pickZ += startZ;

        agent.SetDestination(new Vector3(pickX, 0.79f, pickZ));
    }


    float currentAngle;

    public void AttackStuff()
    {
        currentAngle += Time.deltaTime * attackSpeed;
        weapon.transform.RotateAround(transform.position, transform.right, Time.deltaTime * attackSpeed);

        if(currentAngle > 180f)
        {
            DoDamage();
            currentAngle = 0f;
            weapon.transform.localEulerAngles = new Vector3(0f,0f,180f);
            weapon.transform.localPosition = new Vector3(0f, 0f, 1f);
            attacking = false;
            coolDown = true;
        }
    }

    float currentAngle2;

    public void PullBackWeapon()
    {
        currentAngle2 += Time.deltaTime * pullBackSpeed;
        weapon.transform.RotateAround(transform.position, transform.right, -Time.deltaTime * pullBackSpeed);

        if (currentAngle2 > 180f)
        {
            currentAngle2 = 0f;
            weapon.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
            weapon.transform.localPosition = new Vector3(0f, 0f, -1f);
            coolDown = false;
            agent.Resume();
        }
    }

    public void DoDamage()
    {
        hurtBox.SetActive(true);
        hurtBox.GetComponent<HurtMeGood>().cooldown = hurtTime;
    }
    
}
