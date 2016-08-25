using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureReader : MonoBehaviour {

    List<Point> currentPoints = new List<Point>();
    Gesture[] trainers;
    // PointCloudRecognizer pcr;
    public GameObject testObj;
    public Material mat1;
    public Material mat2;

    void Start()
    {
        //pcr = new PointCloudRecognizer();

        Point vl1 = new Point(0, 0, 0);
        Point vl2 = new Point(0, 100, 0);

        Point[] vA = { vl1, vl2 };

        Point hl1 = new Point(0, 0, 0);
        Point hl2 = new Point(100, 0, 0);

        Point[] hA = { hl1, hl2 };

        Gesture vLine = new Gesture(vA, "verLine");
        Gesture hLine = new Gesture(hA, "hozLine");


        trainers = new Gesture[]{vLine, hLine};
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch currentTouches in Input.touches)
        {
            if(currentTouches.phase == TouchPhase.Began)
            {
                //start recording points
                currentPoints.Clear();
                Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                currentPoints.Add(cPoint);

            } else if (currentTouches.phase == TouchPhase.Ended)
            {
                //send to PDollar
                Point[] pointArray = currentPoints.ToArray();
                Gesture myGesture = new Gesture(pointArray);
                string nameOfShape = PointCloudRecognizer.Classify(myGesture, trainers);

                if(nameOfShape == "verLine")
                {
                    testObj.GetComponent<MeshRenderer>().material = mat1;
                } else
                {
                    testObj.GetComponent<MeshRenderer>().material = mat2;
                }
            } else
            {
                //record point
                Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                currentPoints.Add(cPoint);
            }
        }
    }
}
