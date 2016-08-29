using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GestureReader : MonoBehaviour {

    List<Point> currentPoints = new List<Point>();
    Point firstPoint;
    Gesture[] trainers;
    // PointCloudRecognizer pcr;
    public GameObject testObj;
    public Material mat1;
    public Material mat2;

    float grace;

    int wallMask = 1 << 12;

    public GameObject lo1;
    public GameObject lo2;

    public GameObject to1;

    void Start()
    {
        grace = Screen.height / 15f;

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

    void TapAction(Ray rayT)
    {
        RaycastHit hit;

        if (Physics.Raycast(rayT, out hit, 100f, wallMask))
        {
            to1.transform.position = hit.point;
        }
    }

    void LineAction(Ray startRay, Ray endRay)
    {
        RaycastHit hit1;
        RaycastHit hit2;

        if (Physics.Raycast(startRay, out hit1, 100f, wallMask))
        {
            lo1.transform.position = hit1.point;
        }

        if (Physics.Raycast(endRay, out hit2, 100f, wallMask))
        {
            lo2.transform.position = hit2.point;
        }
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
                firstPoint = cPoint;

            } else if (currentTouches.phase == TouchPhase.Ended)
            {
                Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                currentPoints.Add(cPoint);

                bool isTap = true;

                //do tap test
                Point lastPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                foreach (Point p in currentPoints)
                {
                    if(Geometry.EuclideanDistance(p, lastPoint) > grace)
                    {
                        Debug.Log("Not a tap");
                        isTap = false;
                        break;
                    }
                }

                if(isTap)
                {
                    var rayT = Camera.main.ScreenPointToRay(new Vector3(lastPoint.X, lastPoint.Y, 0f));

                    TapAction(rayT);
                } else
                {
                    //create training line

                    Point[] tempLine = { firstPoint, lastPoint };
                    Gesture tempGest = new Gesture(tempLine, "tLine");

                    Gesture[] tempTrainers = { trainers[0], trainers[1], tempGest };

                    //send to PDollar
                    Point[] pointArray = currentPoints.ToArray();
                    Gesture myGesture = new Gesture(pointArray);
                    string nameOfShape = PointCloudRecognizer.Classify(myGesture, tempTrainers);

                    if(nameOfShape == "tLine")
                    {
                        var ray1 = Camera.main.ScreenPointToRay(new Vector3(firstPoint.X, firstPoint.Y, 0f));
                        var ray2 = Camera.main.ScreenPointToRay(new Vector3(lastPoint.X, lastPoint.Y, 0f));
                        LineAction(ray1, ray2);
                    } else if (nameOfShape == "verLine")
                    {
                        testObj.GetComponent<MeshRenderer>().material = mat1;
                    }
                    else
                    {
                        testObj.GetComponent<MeshRenderer>().material = mat2;
                    }


                }


                //do line test

                
            } else
            {
                //record point
                Point cPoint = new Point(currentTouches.position.x, currentTouches.position.y, 0);
                currentPoints.Add(cPoint);
            }
        }
    }
}
