using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Battle;
using Titanium.Entities;

namespace Titanium.Utilities
{
    class FileUtils
    {
        public FileUtils()
        {

        }

        public List<UnitStats> FileToSprite(String path)
        {
            List<UnitStats> result = new List<UnitStats>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while((line = file.ReadLine())!=null)
            {
                char[] delimiters = new char[] { ',' };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                UnitStats u = new UnitStats();
                u.init(parts);
                result.Add(u);
            }
            return result;
        }

        public static Sprite CreateNewSprite(String path)
        {
            Sprite result = new Sprite(new List<Sprite.SpriteAction>());
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("Content/Stats/NPCStats.txt");
            while ((line = file.ReadLine()) != null)
            {
                char[] delimiters = new char[] { ',' };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                UnitStats u = new UnitStats();
                u.init(parts);
                if(u.model.CompareTo(path)==0)
                {
                    result.setParam(u, 0, 0);
                    return result;
                }
            }
            return result;
        }

        /// <summary>
        /// This function loads battle configuration possibilities from a file.
        /// </summary>
        /// <returns>A dictionary of battle configurations</returns>
        public static Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>> LoadBattleConfigurations()
        {
            Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>> battleConfigs = new Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>>();

            string line;
            char[] delimiters = new char[] { '~', ' ' };

            System.IO.StreamReader file = new System.IO.StreamReader("Content/Stats/BattleConfigs.txt");

            // Loop through the file
            while ((line = file.ReadLine()) != null)
            {
                string[] parts = line.Split(delimiters);

                // If this is an invalid entry
                if (parts.Length < 5)
                {
                    continue;
                }

                // Determine the colliding enemy type
                PartyUtils.Enemy collideType = stringToEnemy(parts[0]);

                // Invalid collision type
                if (collideType == PartyUtils.Enemy.Empty)
                {
                    continue;
                }

                if (!battleConfigs.ContainsKey(collideType))
                {
                    battleConfigs.Add(collideType, new List<List<PartyUtils.Enemy>>());
                }

                List<PartyUtils.Enemy> addList = new List<PartyUtils.Enemy>();
                battleConfigs[collideType].Add(addList);

                // Add the parts to a new list
                for (int i = 1; i < parts.Length; i++)
                {
                    addList.Add(stringToEnemy(parts[i]));
                }
            }

            return battleConfigs;
        }

        /// <summary>
        /// This function converts a string to an enemy enum.
        /// </summary>
        /// <param name="enemy">The enemy string</param>
        /// <returns>The enemy enum</returns>
        private static PartyUtils.Enemy stringToEnemy(string enemy)
        {
            string check = enemy.ToLower();

            if (check.Equals("bat"))
            {
                return PartyUtils.Enemy.Bat;
            }
            else if (check.Equals("rbat"))
            {
                return PartyUtils.Enemy.Redbat;
            }
            else if (check.Equals("slime"))
            {
                return PartyUtils.Enemy.Slime;
            }
            else if (check.Equals("pslime"))
            {
                return PartyUtils.Enemy.PoisonSlime;
            }
            else if (check.Equals("spider"))
            {
                return PartyUtils.Enemy.Spider;
            }
            else if (check.Equals("cspider"))
            {
                return PartyUtils.Enemy.CinderSpider;
            }
            else if (check.Equals("boss"))
            {
                return PartyUtils.Enemy.Boss;
            }
            else if (check.Equals("empty"))
            {
                return PartyUtils.Enemy.Empty;
            }

            return PartyUtils.Enemy.Empty;
        }
    }

}
