using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Battle
{
    class UnitStats
    {
        public String name;
        public String model;
        public int modelFrameCount;
        public int level;
        public int strength;
        public int agility;
        public int intelligence;
        public int baseAttack;
        public int baseHP;
        public int baseMP;
        public int baseSpeed;
        public int currentHP;
        public int currentMP;

        public UnitStats()
        {

        }

        public void init(string[] s)
        {
            name = s[0];
            model = s[1];
            modelFrameCount = Convert.ToInt32(s[2]);
            level = Convert.ToInt32(s[3]);
            strength = Convert.ToInt32(s[4]);
            agility = Convert.ToInt32(s[5]);
            intelligence = Convert.ToInt32(s[6]);
            baseAttack = Convert.ToInt32(s[7]);
            baseHP = Convert.ToInt32(s[8]);
            baseMP = Convert.ToInt32(s[9]);
            baseSpeed = Convert.ToInt32(s[10]);
            normalize();
        }

        public void normalize()
        {
            currentHP = baseHP;
            currentMP = baseMP;
        }
    }
}