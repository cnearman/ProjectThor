using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TouchProcessor
    {
        private float tapGrace = 0.002f; //Temporary, to be changed.

        private IList<Gesture> trainers;

        public TouchProcessor()
        {
            Initialize();
        }

        public void Initialize()
        {
            tapGrace = Screen.height / 15f;
            List<Point> circlePoints = new List<Point>();

            for (float i = 0; i < 360f; i += 11.25f)
            {
                float x = 10f * Mathf.Cos(i);
                float y = 10f * Mathf.Sin(i);

                Point tempC = new Point(x, y, 0);

                circlePoints.Add(tempC);
            }

            Gesture rCircle = new Gesture(circlePoints.ToArray(), "aCircle");

            // ArrowUpPoints
            List<Point> arrowUpPoints = new List<Point>() { new Point(-10, -10, 0), new Point(0, 10, 0), new Point(10, -10, 0) };
            Gesture arrowUp = new Gesture(arrowUpPoints.ToArray(), "aArrowUp");

            trainers = new Gesture[] { rCircle, arrowUp };
        }

        public BaseTouch ProcessPoints(IEnumerable<Point> points, Point firstPoint, Point lastPoint)
        {
            bool isTap = true;
            foreach (Point p in points)
            {
                if (Geometry.EuclideanDistance(p, lastPoint) > tapGrace)
                {
                    Debug.Log("Not a tap");
                    isTap = false;
                    break;
                }
            }

            if (isTap)
            {
                return new Tap()
                {
                    Location = firstPoint.metadata.Location,
                    TappedEntity = firstPoint.metadata.TappedEntity
                };
            }
            else // Not Tap
            {
                Point[] tempLine = { firstPoint, lastPoint };
                Gesture lineGesture = new Gesture(tempLine, "tLine");

                Gesture[] tempTrainers = { trainers[0], lineGesture };

                //send to PDollar
                Point[] pointArray = points.ToArray();
                Gesture myGesture = new Gesture(pointArray);
                string nameOfShape = PointCloudRecognizer.Classify(myGesture, tempTrainers);

                if (nameOfShape == "aCircle")
                {
                    return new CircleGesture();
                }
                else if (nameOfShape == "tLine")
                {
                    return new LineGesture()
                    {
                            StartingPoint = new Vector3(firstPoint.X, firstPoint.Y, 0f),
                            EndingPoint = new Vector3(lastPoint.X, lastPoint.Y, 0f)
                    };
                }
                else
                {
                    throw new Exception("How did you fuck this up so badly");
                }
            }
        }
    }
}
