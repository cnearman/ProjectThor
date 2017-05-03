using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPC : MonoBehaviour {

    PhotonView m_PhotonView;

    bool isAttacking;
    Vector3 pos;
    float currentAngle;
    public float attackSpeed;
    public GameObject myWeapon;
    public AudioSource swordSwipe;

    // Use this for initialization
    void Start () {
        m_PhotonView = GetComponent<PhotonView>();
    }

    public void Attack(Vector3 aPos)
    {
        m_PhotonView.RPC("AttackRPC", PhotonTargets.All, aPos);
    }

    [PunRPC]
    void AttackRPC(Vector3 enemyPos)
    {
        Debug.Log("attack!");
        if(!m_PhotonView.isMine)
        {
            isAttacking = true;
            StartAttack(enemyPos);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (isAttacking)
        {
            currentAngle += Time.deltaTime * attackSpeed;
            myWeapon.transform.RotateAround(transform.position, transform.up, -Time.deltaTime * attackSpeed);

            if (currentAngle > 180f)
            {
                isAttacking = false;
                myWeapon.SetActive(false);
                myWeapon.GetComponentInChildren<PlayerSword>().MakeItSplat(pos);
            }
        }
    }

    void StartAttack(Vector3 pos)
    {
        transform.LookAt(pos);
        myWeapon.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
        currentAngle = 0f;
        swordSwipe.Play();
        myWeapon.SetActive(true);
    }
}
