using UnityEngine;
using System.Collections;
using Assets.Scripts;
using System;

public class UrukHai : BaseEntity, Damagable {
    public AudioSource swiper;

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

    public bool warmUp;
    public float warmUpTime;
    public float currentWarmUp;

    public bool recover;
    public float recoverTime;
    public float currentRecover;

    public bool coolDown;
    public float attackCooldown;
    public float currentCooldown;
    public GameObject weapon;

    public float attackSpeed;
    public float pullBackSpeed;

    public GameObject testPlayer;

    bool isAggro;

    NavMeshAgent agent;

    public GameObject hurtBox;
    public float hurtTime;

    int wallMask = 1 << 12;

    // Use this for initialization
    [OnStart]
    void UrukHaiStart () {
        agent = GetComponent<NavMeshAgent>();

        startX = transform.position.x;
        startZ = transform.position.z;

        currentAngle = 0;
    }
	
	// Update is called once per frame
    [OnUpdate]
	void UrukHaiUpdate () {
        


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
        } else
        {
            
            if (warmUp)
            {
                if(currentWarmUp > 0f)
                {
                    transform.LookAt(testPlayer.transform);
                    currentWarmUp -= Time.deltaTime;
                } else
                {
                    warmUp = false;
                    attacking = true;
                    swiper.Play();
                }
            } else if (coolDown)
            {
                PullBackWeapon();
            } else if (recover)
            {
                if(currentRecover > 0f)
                {
                    agent.SetDestination(testPlayer.transform.position);
                    currentRecover -= Time.deltaTime;
                } else
                {
                    recover = false;
                }
            } else if(attacking)
            {
                AttackStuff();
            } else
            {
                Vector3 tempDist = testPlayer.transform.position - transform.position;
                float acDist = tempDist.magnitude;
                if (acDist < attackDistance)
                {
                    agent.Stop();

                    //transform.LookAt(testPlayer.transform);
                    warmUp = true;
                    currentWarmUp = warmUpTime;
                }
                else
                {
                    agent.SetDestination(testPlayer.transform.position);
                }
            }


            
        }
	}

    void PickPatrolPosition()
    {
        float pickX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float pickZ = UnityEngine.Random.Range(-patrolRange, patrolRange);

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
            recover = true;
            currentRecover = recoverTime;
        }
    }

    public void DoDamage()
    {
        hurtBox.SetActive(true);
        hurtBox.GetComponent<HurtMeGood>().cooldown = hurtTime;
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

    public void ApplyDamage(float damage)
    {
        var health = GetComponent<EHealth>();
        health.health -= (int) damage;
    }
}
