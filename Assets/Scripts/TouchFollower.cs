using UnityEngine;
using System.Collections;

public class TouchFollower : MonoBehaviour {
    ParticleSystem _pr;
	// Use this for initialization
	void Start () {
        _pr = GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
               //_pr.Play();
            }
            else if(touch.phase == TouchPhase.Ended)
            { 
               //_pr.Stop();
               //_pr.Clear();
            }

            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane + 1));
        }
    }
}
