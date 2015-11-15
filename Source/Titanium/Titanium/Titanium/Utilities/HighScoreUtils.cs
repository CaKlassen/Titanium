﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;

namespace Titanium.Utilities
{
    public class HighScoreUtils
    {
        /// <summary>
        /// calculates high score based on how much party health
        /// was preserved/lost, as well as how many potions were used during the level.
        /// </summary>
        /// <param name="HealthMultiplier">Score mutltiplier applied to Health at End of Arena.</param>
        /// <param name="PotionMultiplier">Score mutltiplier applied to Potions used.</param>
        /// <returns>The "high score" for the arena</returns>
        public int CalculateHighScore(int HealthMultiplier, int PotionMultiplier)
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

    }
}
