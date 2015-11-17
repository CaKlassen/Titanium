using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Battle;
using Titanium.Entities;
using Titanium.Utilities;

namespace Titanium.Scenes.Panels
{
    public class EnemyPanel:Panel
    {
        List<Sprite> front;
        List<Sprite> back;
        Encounter encounter;

        static int topOffset = 350;
        static int leftOffset = 650;

        static int offsetX = 120;
        static int offsetY = 120;

        static int offsetBack = 250;

        static int DELAY = 2000;
        static int SIZE = 4;

        public bool active = false;

        int currentDelay = 0;

        GameTime start;

        public Sprite this[int key]
        {
            get
            {
                if(key < SIZE/2)
                    return front[key];
                else
                {
                    key = key - SIZE/2;
                    return back[key];
                }
            }
        }

        public EnemyPanel(Encounter e, List<Sprite> front, List<Sprite> back) : base()
        {
            this.front = front;
            this.back = back;
            encounter = e;
        }

        public override void load(ContentManager content, Viewport v)
        {
            base.load(content, v);
            this.Origin = new Vector2(leftOffset, topOffset);

            Vector2 offset = Vector2.Zero;

            foreach (Sprite sprite in front)
            {
                if (sprite != null)
                {
					sprite.Load(content);
                    sprite.move(Origin + offset);
	                
                }
				offset += new Vector2(offsetX, offsetY);
                
            }

            offset = new Vector2(offsetBack, 0);
            foreach (Sprite sprite in back)
            {
                if (sprite != null)
                {
					sprite.Load(content);
                    sprite.move(Origin + offset);
	                
                }
				offset += new Vector2(offsetX, offsetY);            }
        }

        public override void draw(SpriteBatch sb, Effect effect)
        {
            foreach (Sprite sprite in front)
            {
                if (sprite != null)
                    sprite.Draw(sb, effect);
            }

            foreach (Sprite sprite in back)
            {
                if (sprite != null)
                    sprite.Draw(sb, effect);
            }
            base.draw(sb, effect);
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            if(active)
            {
                if (canAct())
                    currentDelay+=gameTime.ElapsedGameTime.Milliseconds;
                if(currentDelay > DELAY)
                    act();
            }

            foreach (Sprite sprite in front)
            {
                if (sprite != null)
                    sprite.Update(gameTime, inputState);
            }

            foreach (Sprite sprite in back)
            {
                if (sprite != null)
                    sprite.Update(gameTime, inputState);
            }

            base.update(gameTime, inputState);
        }


        public bool canAct()
        {
            for (int i = 0; i < SIZE; ++i)
            {
                if (this[i] != null)
                {
                    switch (this[i].currentState)
                    {
                        case Sprite.State.Idle:
                            return true;
                        default:
                            break;
                    }
                }
            }
            active = false;
            return false;
        }

        public void act()
        {
            currentDelay = 0;
            for(int i = 0; i<SIZE; ++i)
            {
                if (this[i] != null)
                {
                    if (this[i].currentState == Sprite.State.Idle)
                    {
                        PartyUtils.testAction(this[i], PartyUtils.getRandomPartyMember(), new Gambits.GambitResult(1f, int.MaxValue));
                        this[i].currentState = Sprite.State.Resting;
                        return;
                    }
                }
            }
            active = false;
        }

        public bool acting()
        {
            return active;
        }

        public void activate(GameTime gameTime)
        {
            active = true;
            start = gameTime;
        }

        public bool dead()
        {
            for (int i = 0; i < SIZE; ++i)
            {
                if (this[i] != null)
                {
                    if (this[i].currentState != Sprite.State.Dead)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
