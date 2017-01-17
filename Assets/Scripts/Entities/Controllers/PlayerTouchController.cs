using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities.Controllers
{
    public class PlayerTouchController : MonoBehaviour
    {
        private TouchManager tm;

        private void Awake()
        {
            TouchManager.RegisterOnTapEventHandler(HandleTapInput);
            TouchManager.RegisterOnLineEventHandler(HandleLineInput);
            TouchManager.RegisterOnCircleEventHandler(HandleCircleInput);
        }

        public void HandleTapInput(Tap tapObject)
        {
            Debug.Log("Tap");
        }

        public void HandleLineInput(LineGesture lineObject)
        {
            Debug.Log("Line");
        }

        public void HandleCircleInput(CircleGesture circleObject)
        {
            Debug.Log("Circle");
        }
    }
}
