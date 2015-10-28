﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Titanium.Battle;
using Titanium.Entities;

namespace Titanium.Utilities
{
    public static class PartyUtils
    {
        public enum Enemy { Prinny, PrinnyBlue, PrinnyRed, Catsaber, CatsaberBlue, CatsaberRed, Empty };

        public static List<Sprite> partyMembers;

        static PartyUtils()
        {
            partyMembers = new List<Sprite>();
        }

        public static List<Sprite> loadPartyMembers()
        {
            using (var reader = File.OpenText(@"Content/Stats/PlayerFile.txt"))
            {
                while (reader.ReadLine() != null)
                {
                    partyMembers.Add(new Sprite());
                }
            }
            loadStats(partyMembers, "PlayerFile.txt");
            return partyMembers;
        }

        public static List<Sprite> getParty() { return partyMembers; }

        public static int[] getPartyHealth()
        {
            int[] result = new int[partyMembers.Count];
            for(int i = 0; i < partyMembers.Count; ++i)
            {
                result[i] = partyMembers[i].getHealth();
            }
            return result;
        }

        public static int[] getPartyMana()
        {
            int[] result = new int[partyMembers.Count];
            for (int i = 0; i < partyMembers.Count; ++i)
            {
                result[i] = partyMembers[i].getMana();
            }
            return result;
        }

        public static int[] inflictPartyDamage(int damage)
        {
            int[] result = new int[partyMembers.Count];
            for (int i = 0; i < partyMembers.Count; ++i)
            {
                partyMembers[i].takeDamage(damage);
                result[i] = partyMembers[i].getHealth();
            }
            return result;
        }

        public static void loadStats(List<Sprite> l, String target)
        {
            String path = "Content/Stats/";
            List<UnitStats> tempList = new List<UnitStats>();
            FileUtils myFileUtil = new FileUtils();
            tempList = myFileUtil.FileToSprite(path + target);
            for (int i = 0; i < l.Count; ++i)
                l[i].setParam(tempList[i], (int)Vector2.Zero.X, (int)Vector2.Zero.Y);
        }

        public static List<Sprite> makeEnemies(Enemy a, Enemy b = Enemy.Empty, Enemy c = Enemy.Empty, Enemy d = Enemy.Empty)
        {
            FileUtils myFileUtil = new FileUtils();
            List<Sprite> result = new List<Sprite>();
            if (a != Enemy.Empty && !a.Equals(null))
                result.Add(myFileUtil.CreateNewSprite(a.ToString()));
            if (b != Enemy.Empty && !b.Equals(null))
                result.Add(myFileUtil.CreateNewSprite(b.ToString()));
            if (c != Enemy.Empty && !c.Equals(null))
                result.Add(myFileUtil.CreateNewSprite(c.ToString()));
            if (d != Enemy.Empty && !d.Equals(null))
                result.Add(myFileUtil.CreateNewSprite(d.ToString()));
            return result;
        }
    }
}
