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
        

        public string name;
        BaseGambit gambit;
        public Sprite target;

        PlayerSprite player;

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
            this.player = player;
            this.name = name;
            this.gambit = gambit;
            this.action = action;
        }

        public BaseGambit execute(Sprite target, GameTime gameTime)
        {
            this.target = target;
            gambit.start(gameTime);
            return gambit;
        }

        public void load(ContentManager content)
        {
            gambit.load(content);
        }

        public MenuItem makeMenuItem(InputAction action)
        {
            return new MenuItem(name, action);
        }

        public void resolve(PlayerSprite player, GambitResult result)
        {
            action(player, target, result);
        }
    }
}
