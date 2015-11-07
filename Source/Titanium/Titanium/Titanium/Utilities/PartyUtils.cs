using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Titanium.Battle;
using Titanium.Entities;
using Titanium.Gambits;

namespace Titanium.Utilities
{
    public static class PartyUtils
    {
        public delegate void PlayerAction(PlayerSprite player, Sprite target, GambitResult gambitResult);

        public static void testAction(PlayerSprite player, Sprite target, GambitResult gambitResult)
        {
            player.hitTarget(target, gambitResult.multiplier);
        }

        static Skill[][] SKILLS =
        {
            new Skill[]{
                new Skill("Fireball", new Combo(), testAction),
                new Skill("Frostbolt", new Combo(), testAction)
            },
            new Skill[]{
                new Skill("Arcane Arrow", new Rotation(), testAction),
                new Skill("Throwing Knife", new Rotation(), testAction)
            },
            new Skill[]{
                new Skill("Bite", new Mash(), testAction),
                new Skill("Claw", new Mash(), testAction)
            }
        };

        public enum Enemy { Bat, Empty };

        static int MAX_ENEMIES = 2;

        public static List<PlayerSprite> partyMembers = new List<PlayerSprite>();

        static PartyUtils()
        {

        }

        public static List<PlayerSprite> loadPartyMembers()
        {
            using (var reader = File.OpenText(@"Content/Stats/PlayerFile.txt"))
            {
                int i = 0;
                while (reader.ReadLine() != null)
                {
                    partyMembers.Add(new PlayerSprite(SKILLS[i].ToList()));
                }
            }
            loadStats(partyMembers.Cast<Sprite>().ToList(), "PlayerFile.txt");
            return partyMembers;
        }

        public static List<PlayerSprite> getParty() { return partyMembers; }

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

        /// <summary>
        /// Heals the party members by a percentage
        /// </summary>
        /// <param name="percentage">percent to heal the party by</param>
        /// <returns></returns>
        public static int[] HealParty(float percentage)
        {
            int[] result = new int[partyMembers.Count];
            for (int i = 0; i < partyMembers.Count; ++i)
            {
                partyMembers[i].heal(percentage);
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
            List<Sprite> result = new List<Sprite>();
            if (a != Enemy.Empty && !a.Equals(null))
                result.Add(FileUtils.CreateNewSprite(a.ToString()));
            if (b != Enemy.Empty && !b.Equals(null))
                result.Add(FileUtils.CreateNewSprite(b.ToString()));
            if (c != Enemy.Empty && !c.Equals(null))
                result.Add(FileUtils.CreateNewSprite(c.ToString()));
            if (d != Enemy.Empty && !d.Equals(null))
                result.Add(FileUtils.CreateNewSprite(d.ToString()));
            return result;
        }

        public static List<Sprite> makeEnemies(List<Enemy> enemies)
        {
            List<Sprite> result = new List<Sprite>();

            for(int i=0; i < MAX_ENEMIES; i++)
            {
                if (enemies[i] == Enemy.Empty)
                    result.Add(null);
                else
                    result.Add(FileUtils.CreateNewSprite(enemies[i].ToString()));
            }
            return result;
        }

        public static Sprite getRandomPartyMember()
        {
            return partyMembers.ElementAt(new Random().Next(partyMembers.Count));
        }

        
    }
}
