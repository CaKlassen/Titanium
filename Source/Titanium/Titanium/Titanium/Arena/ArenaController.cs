﻿using Microsoft.Xna.Framework;
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
        
        private bool playerMoved;
        private int level;
        private int score;
        private Random generator;

        /// <summary>
        /// The default constructor for the arena controller.
        /// </summary>
        public ArenaController()
        {
            instance = this;
            playerMoved = false;
            score = 0;
            level = 1;
        }

        public ArenaController(SaveData data)
        {
            instance = this;
            playerMoved = false;
            score = data.score;
            level = data.level;

            generator = new Random(data.seed);
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
                // Increment the level

                if (level < 9)
                {
                    level++;

                    score += HighScoreUtils.CalculateHighScore(1000, 250);
                    ArenaScene.instance.loadNewArena(getLevelDifficulty());
                }
                else
                {
                    // Save the player's achieved score
                    SaveUtils save = SaveUtils.getInstance();
                    HighscoreData data = save.loadHighScores();
                    HighScoreUtils.updateHighScores(data.highscores, getScore());
                    save.saveHighScores(data.highscores);

                    ArenaScene.instance.SceneManager.setScene(SceneState.endGame, new EndGameScene(true), true);
                }
            }
        }
        
        public Color getAmbience()
        {
            switch(level)
            {
                case 1:
                case 2:
                case 3:
                {
                    return new Color(0.318f, 0.365f, 0.404f, 1);
                }

                case 4:
                case 5:
                case 6:
                {
                    return new Color(0.259f, 0.310f, 0.247f, 1);
                }

                case 7:
                case 8:
                case 9:
                {
                    return new Color(0.30f, 0.263f, 0.239f, 1);
                }

                default:
                {
                    return Color.White;
                }
            }
        }

        public ArenaDifficulty getLevelDifficulty()
        {
            switch(level)
            {
                case 1:
                    return ArenaDifficulty.EASY;

                case 2:
                    return ArenaDifficulty.EASY;

                case 3:
                    return ArenaDifficulty.EASY;

                case 4:
                    return ArenaDifficulty.MEDIUM;

                case 5:
                    return ArenaDifficulty.MEDIUM;

                case 6:
                    return ArenaDifficulty.MEDIUM;

                case 7:
                    return ArenaDifficulty.HARD;

                case 8:
                    return ArenaDifficulty.HARD;

                case 9:
                    return ArenaDifficulty.VERY_HARD;

                default:
                    return ArenaDifficulty.EASY;
            }
        }

        public string getLevelType()
        {
            string type = "";

            switch (ArenaController.instance.getLevel())
            {
                case 1:
                case 2:
                case 3:
                {
                    type = "Caves";
                    break;
                }
                case 4:
                case 5:
                case 6:
                {
                    type = "Sewers";
                    break;
                }
                case 7:
                case 8:
                case 9:
                {
                    type = "Basement";
                    break;
                }
            }

            return type;
        }


        public int getLevelSize()
        {
            switch(level)
            {
                case 1:
                    return 6;

                case 2:
                    return 6;

                case 3:
                    return 8;

                case 4:
                    return 6;

                case 5:
                    return 8;

                case 6:
                    return 8;

                case 7:
                    return 8;

                case 8:
                    return 10;

                case 9:
                    return 6;

                default:
                    return 6;
            }
        }

        public int getDifficultEnemyThreshold()
        {
            switch (level)
            {
                case 1:
                return 90;

                case 2:
                return 80;

                case 3:
                return 70;

                case 4:
                return 90;

                case 5:
                return 80;

                case 6:
                return 70;

                case 7:
                return 70;

                case 8:
                return 600;

                case 9:
                return 0;

                default:
                return 90;
            }
        }

        /// <summary>
        /// This function marks that the player has moved this frame.
        /// </summary>
        public void setMoved()
        {
            playerMoved = true;
        }

        public int getLevel()
        {
            return level;
        }

        public int getScore()
        {
            return score;
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

        public void setGenerator(Random r)
        {
            generator = r;
        }
    }
}
