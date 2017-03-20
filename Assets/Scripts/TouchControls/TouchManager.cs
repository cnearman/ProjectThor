using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Class for managing the Touch Inputs provided by a player.
    /// </summary>
    public class TouchManager : SingletonComponent<TouchManager>
    {
        private List<Point> points;
        private Point firstPoint;
        private Point lastPoint;
        private TouchProcessor tp;

        public delegate void OnTapDelegate(Tap tapObject);
        public delegate void OnLineDelegate(LineGesture lineObject);
        public delegate void OnCircleDelegate(CircleGesture circleObject);
        private static OnTapDelegate OnTap;
        private static OnLineDelegate OnLine;
        private static OnCircleDelegate OnCircle;

        protected override void Awake()
        {
            tp = new TouchProcessor();
            points = new List<Point>();
            Setup();
        }

        void Update()
        {
            PollTouches();   
        }

        /// <summary>
        /// Method to be called on each update to poll current touch points.
        /// </summary>
        private void PollTouches()
        {
            foreach(Touch currentTouch in Input.touches)
            {
                if (currentTouch.phase == TouchPhase.Began)
                {
                    ClearPoints();
                    AddPoint(currentTouch, TouchOrder.First);
                }
                else if (currentTouch.phase == TouchPhase.Ended)
                {
                    AddPoint(currentTouch, TouchOrder.Last);
                    var result = tp.ProcessPoints(points, firstPoint, lastPoint);
                    CallEvent(result);
                }
                else
                {
                    AddPoint(currentTouch);
                }
            }
        }

        public static void RegisterOnTapEventHandler(OnTapDelegate otd)
        {
            OnTap += otd;
        }

        public static void UnregisterOnTapEventHandler(OnTapDelegate otd)
        {
            OnTap -= otd;
        }

        public static void RegisterOnLineEventHandler(OnLineDelegate otd)
        {
            OnLine += otd;
        }

        public static void UnregisterOnLineEventHandler(OnLineDelegate otd)
        {
            OnLine -= otd;
        }

        public static void RegisterOnCircleEventHandler(OnCircleDelegate otd)
        {
            OnCircle += otd;
        }

        public static void UnregisterOnCircleEventHandler(OnCircleDelegate otd)
        {
            OnCircle -= otd;
        }

        /// <summary>
        /// Method for saving touch points to be processed.
        /// </summary>
        /// <param name="currentTouch"></param>
        /// <param name="type"></param>
        private void AddPoint(Touch currentTouch, TouchOrder type = TouchOrder.Middle)
        {
            var point = new Point(currentTouch.position.x, currentTouch.position.y, 0);
            if (type == TouchOrder.First)
            {
                firstPoint = point;
                var metaData = new PointMetadata();
                var screenRay = Camera.main.ScreenPointToRay(currentTouch.position);
                RaycastHit hit;
                if (Physics.Raycast(screenRay, out hit, 100f))
                {
                    metaData.Location = hit.point;
                    var enemy = hit.collider.gameObject.GetComponent<BaseEnemyMob>();
                    if (enemy != null)
                    {
                        metaData.TappedEntity = enemy.gameObject;
                        metaData.HitSuccessful = true;
                    }
                }
                point.metadata = metaData;
            }
            else if (type == TouchOrder.Last)
            {
                lastPoint = point;
            }

            points.Add(point);
        }

        private void CallEvent(BaseTouch bt)
        {
            switch (bt.TouchType)
            {
                case TouchType.Tap:
                    if (OnTap != null)
                    {
                        OnTap.Invoke(bt as Tap);
                    }
                    break;
                case TouchType.Line:
                    if(OnLine != null)
                    {
                        OnLine.Invoke(bt as LineGesture);
                    }
                    break;
                case TouchType.Circle:
                    if (OnCircle != null)
                    {
                        OnCircle.Invoke(bt as CircleGesture);
                    }
                    break;
            }
        }

        /// <summary>
        /// Method for reseting the point information.
        /// </summary>
        private void ClearPoints()
        {
            points.Clear();
            firstPoint = null;
            lastPoint = null;
        }
    }
}
