﻿using System;
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
            System.IO.StreamReader file = new System.IO.StreamReader("Content/Stats/"+path+".txt");
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

        public static Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>> LoadBattleConfigurations()
        {
            Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>> battleConfigs = new Dictionary<PartyUtils.Enemy, List<List<PartyUtils.Enemy>>>();

            string line;
            char[] delimiters = new char[] { '~', '-' };

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

        private static PartyUtils.Enemy stringToEnemy(string enemy)
        {
            string check = enemy.ToLower();

            if (check.Equals("bat"))
            {
                return PartyUtils.Enemy.Bat;
            }
            else if (check.Equals("empty"))
            {
                return PartyUtils.Enemy.Empty;
            }

            return PartyUtils.Enemy.Empty;
        }
    }

}
