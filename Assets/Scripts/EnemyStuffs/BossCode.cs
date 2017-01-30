using UnityEngine;
using System.Collections;

public class BossCode : MonoBehaviour {
    public GameObject bully;
    public float fireRate;
    float currentFireRate;


    public GameObject pontoon1;
    public GameObject pontoon2;
    public GameObject pontoon3;
    public GameObject pontoon4;

    public Material secondMat;

    public float patrolRange;
    float startX;
    float startZ;

    public float patrolUpdate;
    float currentPatrolUpdate;

    public GameObject testPlayer;
    

    NavMeshAgent agent;

    bool inSecondForm;

    int wallMask = 1 << 12;

    // Use this for initialization
    void Start()
    {
        inSecondForm = false;

        agent = GetComponent<NavMeshAgent>();

        startX = -9f;
        startZ = -26f;
    }

    // Update is called once per frame
    void Update()
    {

        if(currentFireRate <= 0f)
        {
            currentFireRate = fireRate;

            int vh = Random.Range(0, 2);

            if (!inSecondForm)
            {
                if (vh == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Instantiate(bully, new Vector3(-20f, 1f, -26f + Random.Range(-5, 5)), Quaternion.Euler(0f, 90f, 0f));
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Instantiate(bully, new Vector3(-9f + Random.Range(-5, 5), 1f, -35f), Quaternion.Euler(0f, 0f, 0f));
                    }
                }
            } else
            {
                if (vh == 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Instantiate(bully, new Vector3(-20f, 1f, -26f + Random.Range(-5, 5)), Quaternion.Euler(0f, 90f, 0f));
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Instantiate(bully, new Vector3(-9f + Random.Range(-5, 5), 1f, -35f), Quaternion.Euler(0f, 0f, 0f));
                    }
                }
            }

        } else
        {
            currentFireRate -= Time.deltaTime;
        }

        if (currentPatrolUpdate < 0f)
        {
            PickPatrolPosition();

            currentPatrolUpdate = patrolUpdate;
        }
        else
        {
            currentPatrolUpdate -= Time.deltaTime;
        }
        
        if(!inSecondForm && pontoon1 == null && pontoon2 == null && pontoon3 == null && pontoon4 == null)
        {
            Debug.Log("second form");
            fireRate = 0.3f;
            inSecondForm = true;
            SecondForm();
        }
    }

    void SecondForm()
    {
        gameObject.GetComponent<MeshRenderer>().material = secondMat;
        agent.speed = 4;
        patrolUpdate = 1.5f;
        gameObject.tag = "Boss";
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
