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
    public class TouchManager
    {
        private List<Point> points;
        private Point firstPoint;
        private Point lastPoint;
        private TouchProcessor tp;

        public List<BaseTouch> touches;

        public TouchManager()
        {
            points = new List<Point>();
            touches = new List<BaseTouch>();
        }

        /// <summary>
        /// Method to be called on each update to poll current touch points.
        /// </summary>
        public void PollTouches()
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
                    tp.ProcessPoints(points, touches, firstPoint, lastPoint);
                }
                else
                {
                    AddPoint(currentTouch);
                }
            }
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
                if (Physics.Raycast(screenRay, out hit, 100f, Globals.WallMask))
                {
                    metaData.Location = hit.point;
                    var enemy = hit.collider.gameObject.GetComponent<BaseEnemy>();
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
