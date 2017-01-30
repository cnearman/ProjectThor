using UnityEngine;
using System.Collections;

public class Skelington : MonoBehaviour
{
    public AudioSource fireSound;

    public float patrolRange;
    float startX;
    float startZ;

    public float patrolUpdate;
    float currentPatrolUpdate;

    public float angerRange;
    public float walkSpeed;
    public float chaseSpeed;
    public float attackDistance;
    public bool attacking;
    

    public bool recover;
    public float recoverTime;
    public float currentRecover;
    

    public GameObject testPlayer;

    bool isAggro;

    NavMeshAgent agent;

    int wallMask = 1 << 12;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        startX = transform.position.x;
        startZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {



        if (!isAggro)
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

            CheckAggro(testPlayer);
        }
        else
        {
            Vector3 tempDist = testPlayer.transform.position - transform.position;
            float acDist = tempDist.magnitude;
            if (acDist < attackDistance && !Physics.Raycast(transform.position, testPlayer.transform.position - transform.position, acDist, wallMask))
            {
                if (recover)
                {
                    if (currentRecover > 0f)
                    {
                        currentRecover -= Time.deltaTime;
                    }
                    else
                    {
                        recover = false;
                    }
                }
                else
                {
                    agent.Stop();
                    agent.ResetPath();

                    transform.LookAt(testPlayer.transform);
                    Fire();
                    recover = true;
                    currentRecover = recoverTime;

                    agent.Resume();
                }
            }
            else
            {
                agent.SetDestination(testPlayer.transform.position);
            }
        }
    }

    public GameObject bullet;

    void Fire()
    {
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

    public void CheckAggro(GameObject target)
    {
        Vector3 tempDist = target.transform.position - transform.position;
        float acDist = tempDist.magnitude;


        if (!(acDist > angerRange) && !Physics.Raycast(transform.position, target.transform.position - transform.position, acDist, wallMask))
        {
            Component halo = GetComponent("Halo");
            halo.GetType().GetProperty("enabled").SetValue(halo, true, null);
            isAggro = true;
            agent.speed = chaseSpeed;
        } /*else
        {
            Component halo = GetComponent("Halo");
            halo.GetType().GetProperty("enabled").SetValue(halo, false, null);
        }*/

    }
}
