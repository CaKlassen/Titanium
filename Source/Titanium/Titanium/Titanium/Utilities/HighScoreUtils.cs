using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;

namespace Titanium.Utilities
{
    public static class HighScoreUtils
    {
        public static int NUM_STORED_SCORES = 10;

        private static int[] TEMPLATE_SCORES = { 1000, 900, 800, 700, 500, 400, 300, 200, 100, 50 };

        /// <summary>
        /// calculates high score based on how much party health
        /// was preserved/lost, as well as how many potions were used during the level.
        /// </summary>
        /// <param name="HealthMultiplier">Score mutltiplier applied to Health at End of Arena.</param>
        /// <param name="PotionMultiplier">Score mutltiplier applied to Potions used.</param>
        /// <returns>The "high score" for the arena</returns>
        public static int CalculateHighScore(int HealthMultiplier, int PotionMultiplier)
        {
            int score = 0;

            int partyCurHPsum = 0;
            int partyTotalHPsum = 0;

            for(int i = 0; i < PartyUtils.partyMembers.Count; i++)
            {
                partyCurHPsum += PartyUtils.partyMembers[i].getHealth();
                partyTotalHPsum += PartyUtils.partyMembers[i].getBaseHP();
            }

            score = ((partyCurHPsum / partyTotalHPsum) * HealthMultiplier) + (ArenaScene.instance.potionsUsed * PotionMultiplier);

            //save Arena level score

            return score;
        }


        /// <summary>
        /// This function generates a template high score list for first-time players.
        /// </summary>
        /// <returns>A template high score list</returns>
        public static List<int> createInitialHighScores()
        {
            List<int> templateHighScore = new List<int>();

            for (int i = 0; i < TEMPLATE_SCORES.Length; i++)
            {
                templateHighScore.Add(TEMPLATE_SCORES[i]);
            }

            return templateHighScore;
        }

        /// <summary>
        /// This function updates the high score list with a new score. If the new score is high enough
        /// to place in the list, it will be inserted, otherwise it will not.
        /// </summary>
        /// <param name="highScoreList">The current high score list</param>
        /// <param name="newScore">The new score to attempt to insert</param>
        public static void updateHighScores(List<int> highScoreList, int newScore)
        {
            for (int i = 0; i < NUM_STORED_SCORES; i++)
            {
                // If the new score is greater than this score
                if (newScore > highScoreList[i])
                {
                    // Insert the new score
                    highScoreList.Insert(i, newScore);
                    
                    // Remove the bottom score
                    highScoreList.RemoveAt(10);

                    break;
                }
            }
        }

    }
}
