using UnityEngine;
using System.Collections;

public class GoalTroll : MonoBehaviour {

    public GameObject SuperDoor;
    public Light theLight;
    public GameObject boss;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Trololol();
        }
    }

    void Trololol()
    {
        SuperDoor.SetActive(true);
        theLight.intensity = 0;
        Instantiate(boss, new Vector3(-15f,3f,-30f), transform.rotation);
        Destroy(gameObject);
    }
}
