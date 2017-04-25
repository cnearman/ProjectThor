using UnityEngine;
using System.Collections;

public class Skelington : BaseEnemyMob
{
    public AudioSource fireSound;

    public float patrolRange;
    float startX;
    float startZ;

    public float patrolUpdate;
    float currentPatrolUpdate;

    UnityEngine.AI.NavMeshAgent agent;

    // Use this for initialization
    protected override void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        startX = transform.position.x;
        startZ = transform.position.z;

        currentState = possibleState.Patrol;
        currentTarget = GameObject.Find("Player");
    }

    public GameObject bullet;

    protected override void Patrol()
    {
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

    protected override void HighAlertAction()
    {

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
        agent.SetDestination(currentTarget.transform.position);
    }

    protected override void StartAttack()
    {
        transform.LookAt(currentTarget.transform.position);
        fireSound.Play();
        GameObject tempBull = (GameObject)Instantiate(bullet, transform.position, transform.rotation);
    }

    void PickPatrolPosition()
    {
        float pickX = Random.Range(-patrolRange, patrolRange);
        float pickZ = Random.Range(-patrolRange, patrolRange);

        pickX += startX;
        pickZ += startZ;

        agent.SetDestination(new Vector3(pickX, 0.79f, pickZ));
    }
}
