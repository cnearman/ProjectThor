using UnityEngine;
using System.Collections;

public class TouchControl : MonoBehaviour {

    public GameObject testPlayer;
    int wallMask = 1 << 12;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Touch currentTouches in Input.touches)
        {
            //Debug.Log("touched");
            if (currentTouches.phase == TouchPhase.Ended)
            {
                var ray = Camera.main.ScreenPointToRay(currentTouches.position);
                MoveTo(ray);
            }
        }
    }

    void MoveTo(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, wallMask))
        {
            testPlayer.transform.position = hit.point;
        }
    }
}
