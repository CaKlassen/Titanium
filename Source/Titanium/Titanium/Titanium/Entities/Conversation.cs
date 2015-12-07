using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Utilities;

namespace Titanium.Entities
{
    public class Conversation : Entity
    {
        private static int MOVE_RATE = 10;

        private InputAction skip = InputAction.B;
        private List<Textbox> textboxes;
        private bool done;
        private Texture2D bg;
        private float offset = 0;

        public Conversation()
        {
            textboxes = new List<Textbox>();
            done = false;
        }

        public void load(ContentManager Content)
        {
            bg = Content.Load<Texture2D>("Sprites/Conversation-Background");

            foreach (Textbox tb in textboxes)
            {
                tb.load(Content);
            }
        }

        public void addTextbox(Textbox tb)
        {
            tb.setConversation(this);
            textboxes.Add(tb);
        }

        public void nextTextbox()
        {
            if (textboxes.Count > 0)
            {
                textboxes.RemoveAt(0);
            }
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (!done)
            {
                if (textboxes.Count > 0 && offset == bg.Height)
                {
                    textboxes[0].Update(gameTime, inputState);

                    if (skip.Evaluate(inputState, null, out player))
                    {
                        // Skip all dialogue
                        textboxes.Clear();
                    }
                }

                if (textboxes.Count > 0)
                {
                    if (offset < bg.Height - 1)
                    {
                        offset += MathUtils.smoothChange(offset, bg.Height, MOVE_RATE);
                    }
                    else
                    {
                        offset = bg.Height;
                    }
                }
                else
                {
                    // If we no longer have any textboxes
                    if (offset > 1)
                    {
                        offset += MathUtils.smoothChange(offset, 0, MOVE_RATE);
                    }
                    else
                    {
                        done = true;
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (!done)
            {
                sb.Begin();

                sb.Draw(bg, new Vector2(0, BaseGame.SCREEN_HEIGHT - offset), Color.White);

                sb.End();

                if (textboxes.Count > 0)
                {
                    textboxes[0].Draw(sb, effect);
                }
            }
        }

        public bool getDone()
        {
            return done;
        }

        public override Vector3 getPOSITION()
        {
            throw new NotImplementedException();
        }

    }
}
