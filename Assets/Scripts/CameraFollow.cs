using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public GameObject objectTofollow;
    public float hArea;
    public float vArea;

    public float vOffset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(objectTofollow.transform.position.x > transform.position.x + hArea)
        {
            transform.position = new Vector3(objectTofollow.transform.position.x - hArea, transform.position.y, transform.position.z);
        } else if(objectTofollow.transform.position.x < transform.position.x - hArea)
        {
            transform.position = new Vector3(objectTofollow.transform.position.x + hArea, transform.position.y, transform.position.z);
        }

        if(objectTofollow.transform.position.z > (transform.position.z + vOffset) + vArea)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, objectTofollow.transform.position.z - vOffset - vArea);
        } else if(objectTofollow.transform.position.z < (transform.position.z + vOffset) - vArea)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, objectTofollow.transform.position.z - vOffset + vArea);
        }
	}
}
