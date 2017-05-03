using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseRPC : MonoBehaviour {

    PhotonView m_PhotonView;
    PhotonTransformView m_PhotonTransformView;

    float health;

    public void Kill()
    {
        m_PhotonView.RPC("KillEnemy", PhotonTargets.All);
    }

    public void Damage()
    {
        m_PhotonView.RPC("DamageEnemy", PhotonTargets.All);
    }

    [PunRPC]
    void DamageEnemy()
    {
        health -= 1;
        if(health <= 0)
        {
            Kill();
        }
    }

    [PunRPC]
    void KillEnemy()
    {
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
        m_PhotonView = GetComponent<PhotonView>();
        m_PhotonTransformView = GetComponent<PhotonTransformView>();
        health = 3;
    }
	
	// Update is called once per frame
	void Update () {
        m_PhotonTransformView.SetSynchronizedValues(GetComponent<Rigidbody>().velocity, 0);
    }
}
