using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Battle;
using Titanium.Entities;
using Titanium.Gambits;
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

        static int DELAY = 1500;
        static int SIZE = 4;

        public bool active = false;
        bool gambitActive = false;

        int currentDelay = 0;

        GameTime start;

        GambitResult result;
        Counter counterGambit;

        Sprite actingSprite;

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
            counterGambit = new Counter();
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
				offset += new Vector2(offsetX, offsetY);
            }

            counterGambit.load(content);
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
                {
                    if(gambitActive)
                    {
                        if (encounter.battleMenu.gambitComplete(out result))
                        {
                            gambitActive = false;
                            act(result);
                        }
                            
                    }
                    else
                    {
                        if (currentDelay > DELAY)
                        {
                            encounter.battleMenu.start(counterGambit, actingSprite.getStats().difficulty, gameTime);
                            gambitActive = true;
                        }
                        else
                            currentDelay += gameTime.ElapsedGameTime.Milliseconds;
                        
                    }
                }
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
                            actingSprite = this[i];
                            return true;
                        default:
                            break;
                    }
                }
            }
            active = false;
            return false;
        }

        public void act(GambitResult result)
        {
            currentDelay = 0;
            for(int i = 0; i<SIZE; ++i)
            {
                if (this[i] != null)
                {
                    if (this[i].currentState == Sprite.State.Idle)
                    {
                        PartyUtils.testAction(this[i], PartyUtils.getRandomPartyMember(), result);
                        return;
                    }
                }
            }
        }

        public bool acting()
        {
            for (int i = 0; i < SIZE; ++i)
            {
                if(this[i] != null)
                    switch(this[i].currentState)
                    {
                        case Sprite.State.Resting:
                        case Sprite.State.Dead:
                            continue;
                        default:
                            return true;
                    }
            }
            return false;
        }

        public void activate(GameTime gameTime)
        {
            for (int i = 0; i < SIZE; ++i)
            {
                if (this[i]!= null)
                {
                    if (this[i].currentState != Sprite.State.Dead)
                        this[i].currentState = Sprite.State.Idle;
                }
            }
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

        public bool inGambit()
        {
            return gambitActive;
        }
    }
}
