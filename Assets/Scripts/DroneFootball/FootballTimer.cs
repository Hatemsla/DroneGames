using UnityEngine;

namespace Drone.DroneFootball
{
    public sealed class FootballTimer : Timer
    {
        private void Start()
        {
            waitForEndGame = timeForEndGame;
            waitForStartGame = timeForStartGame;
        }

        private void Update()
        {
            WaitForEndGame();
            WaitForStartGame();
        }

        private void WaitForEndGame()
        {
            if (waitForEndGame >= 0 && waitForStartGame <= 0)
            {
                waitForEndGame -= Time.deltaTime;
            }
        }
        
        public override void ResetTimeScale()
        {
            
        }

        public override void WaitForStartGame()
        {
            if (waitForStartGame >= 0)
            {
                waitForStartGame -= Time.deltaTime;
            }
        }
    }
}