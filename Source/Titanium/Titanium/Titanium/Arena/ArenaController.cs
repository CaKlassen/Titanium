using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;
using Titanium.Utilities;

namespace Titanium.Entities
{
    /// <summary>
    /// The arena controller, responsible for managing arena state.
    /// </summary>
    public class ArenaController
    {
        public static ArenaController instance;

        private ArenaDifficulty curDifficulty;
        private bool playerMoved;
        private Random generator;

        /// <summary>
        /// The default constructor for the arena controller.
        /// </summary>
        public ArenaController()
        {
            instance = this;
            playerMoved = false;
            curDifficulty = ArenaDifficulty.EASY;

            generator = new Random();
        }

        /// <summary>
        /// This function is called every frame.
        /// </summary>
        public void update()
        {
            // The player has no longer moved
            if (playerMoved)
            {
                playerMoved = false;
            }
        }

        public int getNumEnemies()
        {
            List<Entity> collidables = ArenaScene.instance.collidables;
            int numEnemies = 0;

            foreach (Entity e in collidables)
            {
                if (e.GetType() == typeof(ArenaEnemy))
                {
                    numEnemies++;
                }
            }

            return numEnemies;
        }

        /// <summary>
        /// This function moves the arena scene to the next arena.
        /// </summary>
        public void moveToNextArena()
        {
            

            if (getNumEnemies() == 0)
            {
                // Increment the difficulty
                if (curDifficulty != ArenaDifficulty.HARD)
                {
                    curDifficulty++;
                }

                ArenaScene.instance.score = HighScoreUtils.CalculateHighScore(10, 10);
                ArenaScene.instance.loadNewArena(curDifficulty);
            }
        }
        

        /// <summary>
        /// This function marks that the player has moved this frame.
        /// </summary>
        public void setMoved()
        {
            playerMoved = true;
        }

        /// <summary>
        /// This function returns the player moved state for this frame.
        /// </summary>
        /// <returns>The player moved state</returns>
        public bool getPlayerMoved()
        {
            return playerMoved;
        }

        /// <summary>
        /// This function returns the arena's Random generator.
        /// </summary>
        /// <returns></returns>
        public Random getGenerator()
        {
            return generator;
        }
    }
}
