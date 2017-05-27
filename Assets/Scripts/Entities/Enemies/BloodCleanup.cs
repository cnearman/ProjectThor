using UnityEngine;
using System.Collections;

public class BloodCleanup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 3f);
	}
}
