using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class OnHealthChangedArgs : EventArgs
    {
        public float ChangeValue;
        public GameObject StatusEffect;
        public bool Cancel;

        public OnHealthChangedArgs(float changeValue, GameObject statusEffect)
        {
            ChangeValue = changeValue;
            StatusEffect = statusEffect;
            Cancel = false;
        }
    }
}
