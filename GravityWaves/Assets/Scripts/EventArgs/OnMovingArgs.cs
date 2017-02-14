using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class OnMovingArgs : EventArgs
    {
        public Vector3 Velocity;
        public bool Cancel;

        public OnMovingArgs(Vector3 velocity)
        {
            Velocity = velocity;
            Cancel = false;
        }
    }
}
