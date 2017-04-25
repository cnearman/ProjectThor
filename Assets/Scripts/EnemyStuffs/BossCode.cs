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
    

    UnityEngine.AI.NavMeshAgent agent;

    bool inSecondForm;

    int wallMask = 1 << 12;

    // Use this for initialization
    void Start()
    {
        inSecondForm = false;

        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        startX = -24f;
        startZ = -11f;
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
                        var range = Random.Range(0, 9) + 1;
                        var instance = (GameObject)Instantiate(bully, new Vector3(-25.66f, 0.5f, -1.25f) + new Vector3((-14.18f - (-25.66f)) * (range / 10.0f), 0.5f, (-11.25f - -1.25f) * (range / 10.0f)), Quaternion.identity);
                        instance.transform.forward = new Vector3(-35.4f, 0.5f, -11.25f) - new Vector3(-25.66f, 0.5f, -1.25f);
                    }
                }
                else
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var range = Random.Range(0, 9) + 1;
                        var instance = (GameObject)Instantiate(bully, new Vector3(-25.66f, 0.5f, -1.25f) + new Vector3((-35.4f - (-25.66f)) * (range / 10.0f), 0.5f, (-11.25f - -1.25f) * (range / 10.0f)), Quaternion.identity);
                        instance.transform.forward = new Vector3(-14.18f, 0.5f, -12.73f) - new Vector3(-25.66f, 0.5f, -1.25f);
                    }
                }
            } else
            {
                if (vh == 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        var range = Random.Range(0, 9) + 1;
                        var instance = (GameObject) Instantiate(bully, new Vector3(-25.66f, 0.5f, -1.25f) + new Vector3((-14.18f - (-25.66f)) * (range / 10.0f), 0.5f, (-11.25f - -1.25f) * (range / 10.0f)), Quaternion.identity);
                        instance.transform.forward = new Vector3(-35.4f, 0.5f, -11.25f) - new Vector3(-25.66f, 0.5f, -1.25f);
                    }
                }
                else
                {
                    for (int i = 0; i < 1; i++)
                    {
                        var range = Random.Range(0, 9) + 1;
                        var instance = (GameObject) Instantiate(bully, new Vector3(-25.66f, 0.5f, -1.25f) + new Vector3((-35.4f - (-25.66f)) * (range / 10.0f), 0.5f, (-11.25f - -1.25f) * (range / 10.0f)), Quaternion.identity);
                        instance.transform.forward = new Vector3(-14.18f, 0.5f, -12.73f) - new Vector3(-25.66f, 0.5f, -1.25f);
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
