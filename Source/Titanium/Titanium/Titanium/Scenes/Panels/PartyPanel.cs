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

        static float topOffset = 0.2f;
        static float leftOffset = 0f;

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
            this.Origin = new Vector2(v.Width * leftOffset, v.Height * topOffset);
            Vector2 offset = new Vector2();

            for (int i= 0; i < party.Count; ++i)
            {
                party[i].Load(content);
                party[i].move(Origin + offset);
                offset += new Vector2(0, party[i].getHeight());
            }
        }

        public override void draw(SpriteBatch sb)
        {
            foreach (PlayerSprite player in party)
                player.Draw(sb);

            base.draw(sb);
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
            foreach (PlayerSprite sprite in party)
                if (sprite.currentState != Sprite.State.Resting)
                    return false;
            return true;
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

    }
}
