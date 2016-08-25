using UnityEngine;
using System.Collections;

public class Cursor : MonoBehaviour {

    public ParticleSystem bloom;

    public void PlayPart()
    {
        bloom.Play();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
