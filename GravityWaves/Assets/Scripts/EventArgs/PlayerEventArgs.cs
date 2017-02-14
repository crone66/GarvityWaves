using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerEventArgs : EventArgs
    {
        public GameObject PlayerObject;
        public Player PlayerScript;

        public PlayerEventArgs(GameObject playerObject, Player playerScript)
        {
            PlayerObject = playerObject;
            PlayerScript = playerScript;
        }
    }
}
