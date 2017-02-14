using System;
using System.Collections.Generic;
using XInputDotNetPure;

namespace Assets.Scripts
{
    public static class GamePadManager
    {
        private static bool[] usedIndices = new bool[4] { false, false, false, false };

        public static void Connect(int padIndex)
        {
            if(padIndex >= 0 && padIndex <= 4)
                usedIndices[padIndex] = true;
        }

        public static bool IsInUse(int padIndex)
        {
            if (padIndex >= 0 && padIndex <= 4)
                return usedIndices[padIndex];
            else
                throw new ArgumentOutOfRangeException();        
        }

        public static void Disconnect(PlayerIndex playerIndex)
        {
            int index = (int)playerIndex;
            if (index >= 0 && index <= 4)
                usedIndices[index] = false;
        }

        public static void DisconnectAll()
        {
            Disconnect(PlayerIndex.One);
            Disconnect(PlayerIndex.Two);
            Disconnect(PlayerIndex.Three);
            Disconnect(PlayerIndex.Four);
        }

        public static PlayerIndex GetPlayerIndex()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!IsInUse(i))
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Connect(i);
                        return testPlayerIndex;
                    }
                }
            }

            throw new Exception("Free GamePad not found!");
        }

        public static PlayerIndex[] GetFreeControllers()
        {
            List<PlayerIndex> availibleControllers = new List<PlayerIndex>();
            for (int i = 0; i < 4; i++)
            {
                if (!IsInUse(i))
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        availibleControllers.Add(testPlayerIndex);
                    }
                }
            }

            return availibleControllers.ToArray();
        }
    }
}
