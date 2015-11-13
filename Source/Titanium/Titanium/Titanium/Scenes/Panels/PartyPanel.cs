using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Titanium.Utilities;
using Titanium.Battle;

namespace Titanium.Scenes.Panels
{
    public class PartyPanel:Panel
    {
        List<PlayerSprite> party;
        Encounter encounter;

        static int topOffset = 300;
        static int leftOffset = 300;

        static int offsetX = -100;
        static int offsetY = 100;


        public PlayerSprite this[int key]
        {
            get { return party[key];  }
        }


        public PartyPanel(Encounter e): base()
        {
            party = PartyUtils.getParty();
            this.encounter = e;
        }

        public override void load(ContentManager content, Viewport v)
        {
            base.load(content, v);
            this.Origin = new Vector2(leftOffset, topOffset);
            Vector2 offset = new Vector2();

            for (int i= 0; i < party.Count; ++i)
            {
                party[i].Load(content);
                party[i].move(Origin + offset);
                offset += new Vector2(offsetX, offsetY);
            }
        }

        public override void draw(SpriteBatch sb, Effect effect)
        {
            foreach (PlayerSprite player in party)
                player.Draw(sb, effect);

            base.draw(sb, effect);
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            foreach (PlayerSprite player in party)
                player.Update(gameTime, inputState);

            base.update(gameTime, inputState);
        }

        public bool idle()
        {
            foreach (PlayerSprite sprite in party)
                if (sprite.currentState != Sprite.State.Idle && sprite.currentState != Sprite.State.Resting)
                    return false;
            return true;
        }

        public bool hasActed()
        {
            bool result = true;
            foreach (PlayerSprite sprite in party)
                result &= sprite.currentState == Sprite.State.Resting;
            return result;
        }

        public void activate()
        {
            foreach (PlayerSprite sprite in party)
                sprite.changeState(Sprite.State.Idle);
        }

        public bool dead()
        {
            foreach (PlayerSprite sprite in party)
            {
                if (sprite.currentState != Sprite.State.Dead)
                {
                    return false;
                }
            }
            return true;
        }

        public void reset()
        {
            foreach (PlayerSprite hero in party)
                if (hero.currentState != Sprite.State.Dead)
                    hero.changeState(Sprite.State.Idle);
        }
    }
}
