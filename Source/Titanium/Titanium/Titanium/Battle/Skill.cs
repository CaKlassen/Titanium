using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Gambits;
using Titanium.Scenes.Panels;
using Titanium.Utilities;

namespace Titanium.Battle
{
    public class Skill
    {

        int width = 20;

        public string name;
        public BaseGambit gambit;
        public Sprite target;

        PartyUtils.PlayerAction action;

        public enum Type
        {
            Melee,
            Ranged,
            Magical
        }
        public Type type;

        public Skill(string name, BaseGambit gambit, PartyUtils.PlayerAction action)
        {
            this.name = name;
            this.gambit = gambit;
            this.action = action;
        }

        public void load(ContentManager content)
        {
            gambit.load(content);
        }

        public MenuItem makeMenuItem(InputAction action)
        {
            return new MenuItem(name, gambit, action);
        }


        public void resolve(PlayerSprite player, Sprite target, EnemyPanel enemies, GambitResult result)
        {
            action(player, target, result);
        }
    }
}
