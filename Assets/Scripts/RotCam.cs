using UnityEngine;
using System.Collections;

public class RotCam : MonoBehaviour {

    public float maxRot;

    public float rotRate;



    // Use this for initialization
    void Start()
    {

    }

    float count;

    // Update is called once per frame
    void Update()
    {
        float curDev = maxRot * Mathf.Sin(count);

        Quaternion curRot = Quaternion.Euler(0f, curDev, 0f);

        transform.localRotation = curRot;
        count += Time.deltaTime * rotRate;

        //Debug.Log(curDev);

    }
}
