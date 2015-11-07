using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Utilities;

namespace Titanium.Arena
{
    public class BattleBuilder
    {
        private static Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>> battleConfigs = null;
        private Random r;

        private List<PartyUtils.Enemy> front = new List<PartyUtils.Enemy>();
        private List<PartyUtils.Enemy> back = new List<PartyUtils.Enemy>();

        private PartyUtils.Enemy enemy;

        /// <summary>
        /// This is the constructor for the battle builder.
        /// </summary>
        /// <param name="enemy">The enemy collided with</param>
        public BattleBuilder(PartyUtils.Enemy enemy)
        {
            this.enemy = enemy;

            // Load the battle configurations for the first time
            if (battleConfigs == null)
            {
                battleConfigs = FileUtils.LoadBattleConfigurations();
            }

            r = ArenaController.instance.getGenerator();

            generateBattle();
        }


        /// <summary>
        /// This function generates a random battle
        /// </summary>
        private void generateBattle()
        {
            List<List<PartyUtils.Enemy>> collideList = battleConfigs[enemy];

            // Get a random list from this enemy type
            int randomSelection = r.Next(collideList.Count);
            List<PartyUtils.Enemy> randomList = collideList[randomSelection];

            front.Clear();
            back.Clear();

            // Create the front and back rows
            front.Add(randomList[0]);
            front.Add(randomList[1]);
            back.Add(randomList[2]);
            back.Add(randomList[3]);
        }

        /// <summary>
        /// This function returns the front row of enemies.
        /// </summary>
        /// <returns>The front row of enemies</returns>
        public List<PartyUtils.Enemy> getFront()
        {
            return front;
        }

        /// <summary>
        /// This function returns the back row of enemies.
        /// </summary>
        /// <returns>The back row of enemies</returns>
        public List<PartyUtils.Enemy> getBack()
        {
            return back;
        }
    }
}
