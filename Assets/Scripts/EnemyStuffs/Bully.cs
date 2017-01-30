using UnityEngine;
using System.Collections;

public class Bully : MonoBehaviour {

    public float speed;
    public GameObject bloodHit;

    // Use this for initialization
    void Start () {
        Destroy(gameObject, 3f);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward * Time.deltaTime * speed, Space.World);
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
