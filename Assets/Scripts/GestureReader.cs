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

    public GameObject co1;

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

        List<Point> circlePoints = new List<Point>();

        for(float i = 0; i < 360f; i += 11.25f)
        {
            float x = 10f * Mathf.Cos(i);
            float y = 10f * Mathf.Sin(i);

            Point tempC = new Point(x, y, 0);

            circlePoints.Add(tempC);
        }

        Gesture rCircle = new Gesture(circlePoints.ToArray(), "aCircle");

        trainers = new Gesture[]{vLine, hLine, rCircle};
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

    void DrawCircle(Ray origin, Ray rad)
    {
        RaycastHit hit1;
        RaycastHit hit2;

        float fX = 0f;
        float dX;

        if (Physics.Raycast(origin, out hit1, 100f, wallMask))
        {
            co1.transform.position = hit1.point;
            fX = hit1.point.x;
        }

        if (Physics.Raycast(rad, out hit2, 100f, wallMask))
        {
            dX = Mathf.Abs(fX - hit2.point.x) * 2f;
            co1.transform.localScale = new Vector3(dX, 0.3f, dX);
            
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

                    Gesture[] tempTrainers = { trainers[0], trainers[1], trainers[2], tempGest };

                    //send to PDollar
                    Point[] pointArray = currentPoints.ToArray();
                    Gesture myGesture = new Gesture(pointArray);
                    string nameOfShape = PointCloudRecognizer.Classify(myGesture, tempTrainers);

                    if (nameOfShape == "tLine")
                    {
                        var ray1 = Camera.main.ScreenPointToRay(new Vector3(firstPoint.X, firstPoint.Y, 0f));
                        var ray2 = Camera.main.ScreenPointToRay(new Vector3(lastPoint.X, lastPoint.Y, 0f));
                        LineAction(ray1, ray2);
                    } else if (nameOfShape == "aCircle") {
                        Debug.Log("its a circle");
                        //find origin and radius
                        float totalX = 0f;
                        float totalY = 0f;

                        float aveX = 0f;
                        float aveY = 0f;

                        float minY = pointArray[0].Y;
                        float maxY = pointArray[0].Y;

                        float minX = pointArray[0].X;
                        float maxX = pointArray[0].X;

                        for (int j = 0; j < pointArray.Length; j++)
                        {
                            totalX += pointArray[j].X;
                            totalY += pointArray[j].Y;

                            if(pointArray[j].X < minX)
                            {
                                minX = pointArray[j].X;
                            }
                            if(pointArray[j].X > maxX)
                            {
                                maxX = pointArray[j].X;
                            }
                            if(pointArray[j].Y < minY)
                            {
                                minY = pointArray[j].Y;
                            }
                            if(pointArray[j].Y > maxY)
                            {
                                maxY = pointArray[j].Y;
                            }

                        }

                        float myRadius = ((Mathf.Abs((maxX - minX)) + Mathf.Abs((maxY - minY))) / 2f) / 2f;

                        aveX = totalX / pointArray.Length;
                        aveY = totalY / pointArray.Length;

                        Debug.Log(totalX + ", " + aveX);

                        var ray3 = Camera.main.ScreenPointToRay(new Vector3(aveX, aveY, 0f));
                        var ray4 = Camera.main.ScreenPointToRay(new Vector3(aveX + myRadius, aveY, 0f));

                        DrawCircle(ray3, ray4);

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
