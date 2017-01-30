using UnityEngine;
using System.Collections;

public class PlayerSword : MonoBehaviour {

    public GameObject bloodHit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            Instantiate(bloodHit, other.gameObject.transform.position, other.gameObject.transform.rotation);
            other.gameObject.GetComponent<EHealth>().health -= 1;
        }
        if (other.gameObject.tag == "Boss")
        {
            Instantiate(bloodHit, other.gameObject.transform.position, other.gameObject.transform.rotation);
            other.gameObject.GetComponent<BHealth>().health -= 1;
        }
    }
}
