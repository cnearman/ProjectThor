using UnityEngine;
using System.Collections;

public class HurtMeGood : MonoBehaviour {

    public float cooldown;
    public GameObject bloodHit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
        } else
        {
            gameObject.SetActive(false);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Instantiate(bloodHit, other.gameObject.transform.position, other.gameObject.transform.rotation);
            other.gameObject.GetComponent<Healths>().health -= 1;
        }
    }
}
