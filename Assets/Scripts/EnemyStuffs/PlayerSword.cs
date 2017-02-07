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

    public void MakeItSplat(Vector3 pos)
    {
        Debug.Log("It's SPLATTIN");
        Instantiate(bloodHit, pos, Quaternion.identity);
    }
}
