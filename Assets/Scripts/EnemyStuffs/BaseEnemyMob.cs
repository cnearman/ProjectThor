using UnityEngine;
using System.Collections;

public class BaseEnemyMob : MonoBehaviour {

    protected GameObject currentTarget;

    public GameObject chao;
    public Material patrolM;
    public Material moveForAttackM;
    public Material startingAttackM;
    public Material attackM;
    public Material preparingAttackM;
    public Material highAlertM;

    protected enum possibleState { Patrol, MoveForAttack, StartingAttack, Attack, PreparingNextAttack, HighAlert };
    protected possibleState currentState;

    public float aggroRange;
    public float attackRange;
    public float attackWarmUp;
    float currentAttackWarmUp;
    public float attackTime;
    float currentAttackTime;
    public float attackCoolDown;
    float currentAttackCoolDown;
    public float dropAggroRange;
    public float dropAggroTime;
    float currentDropAggroTime;
    public float hightAlertAggroRange;
    public float dropAlertTime;
    float currentDropAlertTime;

    int wallMask = 1 << 12;

    protected virtual void Start()
    {
        currentState = possibleState.Patrol;
        StartPatrol();
    }

    protected virtual void Update()
    {
        if (currentState == possibleState.Patrol)
        {
            chao.GetComponent<MeshRenderer>().material = patrolM;

            //Do patrolling this
            Patrol();

            //Check for LOS and range
            if (LOS(currentTarget, aggroRange))
            {
                //Go to moveForAttack
                currentState = possibleState.MoveForAttack;
                currentDropAggroTime = dropAggroTime;
                StartMoveToAttack();
            }
        }
        else if (currentState == possibleState.MoveForAttack)
        {
            chao.GetComponent<MeshRenderer>().material = moveForAttackM;

            MoveToAttack();

            //Check for LOS and range
            if (LOS(currentTarget, attackRange))
            {
                //Go to start attack, Start timer for attack
                currentState = possibleState.StartingAttack;
                currentAttackWarmUp = attackWarmUp;
                StartPoweringUp();
            }
            else
            {
                //Check for de-aggro
                if (!LOS(currentTarget, dropAggroRange))
                {
                    if (currentDropAggroTime <= 0)
                    {
                        //go to high aleart
                        currentState = possibleState.HighAlert;
                        currentDropAlertTime = dropAlertTime;
                        StartHighAlertAction();
                    }
                    else
                    {
                        currentDropAggroTime -= Time.deltaTime;
                    }
                }
                else
                {
                    currentDropAggroTime = dropAggroTime;
                }
            }
        }
        else if (currentState == possibleState.StartingAttack)
        {
            chao.GetComponent<MeshRenderer>().material = startingAttackM;

            PoweringUp();

            //Dec timer
            if (currentAttackWarmUp <= 0f)
            {
                //Go to attack
                currentState = possibleState.Attack;
                currentAttackTime = attackTime;
                StartAttack();
            }
            else
            {
                currentAttackWarmUp -= Time.deltaTime;
            }
        }
        else if (currentState == possibleState.Attack)
        {
            chao.GetComponent<MeshRenderer>().material = attackM;

            Attack();

            if (currentAttackTime <= 0f)
            {
                //Go to prepare for attack
                currentState = possibleState.PreparingNextAttack;
                currentAttackCoolDown = attackCoolDown;
                StartPrepareNextAttack();
            } else
            {
                currentAttackTime -= Time.deltaTime;
            }

        }
        else if (currentState == possibleState.PreparingNextAttack)
        {
            chao.GetComponent<MeshRenderer>().material = preparingAttackM;

            PrepareNextAttack();

            if (currentAttackCoolDown <= 0)
            {
                currentState = possibleState.MoveForAttack;
                StartMoveToAttack();
            }
            else
            {
                currentAttackCoolDown -= Time.deltaTime;
            }
        }
        else if (currentState == possibleState.HighAlert)
        {
            chao.GetComponent<MeshRenderer>().material = highAlertM;

            HighAlertAction();

            if (currentDropAlertTime <= 0)
            {
                //drop aggro
                currentState = possibleState.Patrol;
                StartPatrol();
            }
            else
            {
                if (LOS(currentTarget, hightAlertAggroRange))
                {
                    //go to move for attack
                    currentState = possibleState.MoveForAttack;
                    currentDropAggroTime = dropAggroTime;
                    StartMoveToAttack();
                }
                else
                {
                    currentDropAlertTime -= Time.deltaTime;
                }
            }

        }
    }


    protected virtual void StartPatrol() { }

    protected virtual void StartMoveToAttack() { }

    protected virtual void StartPoweringUp() { }

    protected virtual void StartPrepareNextAttack() { }

    protected virtual void StartAttack() { }

    protected virtual void StartHighAlertAction() { }



    protected virtual void Patrol() { }

    protected virtual void MoveToAttack() { }

    protected virtual void PoweringUp() { }

    protected virtual void PrepareNextAttack() { }

    protected virtual void Attack() { }

    protected virtual void HighAlertAction() { }

    protected bool LOS(GameObject LOSTarget, float range)
    {
        if(LOSTarget == null)
        {
            return false;
        }
        Vector3 tempDist = LOSTarget.transform.position - transform.position;
        float acDist = tempDist.magnitude;


        if (acDist > range || Physics.Raycast(transform.position, tempDist, acDist, wallMask))
        {
            return false;
        }

        return true;
    }
}
