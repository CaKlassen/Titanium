using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Entities
{
    public enum TextChar
    {
        LEO,
        KLEPTO,
        CLEM,
        VILLAIN
    }

    public class Textbox : Entity
    {
        private static int NAME_OFF = 10;
        private static int TEXT_OFFSET = 40;
        private static int PORTRAIT_OFF = 16;

        private static float SCROLL_RATE = 0.7f;
        private static int WAIT_TIME = 180;
        private static float FADE_RATE = 0.05f;

        private static SpriteFont nameFont;
        private static SpriteFont textFont;

        private string text;
        private TextChar character;
        private Color textColor = Color.White;
        private Color nameColor;
        private string charName;
        private Conversation conversation;
        private Texture2D portrait;
     
        private Vector2 namePos;
        private Vector2 textPos;
        private string partText = "";
        private float pos = 1;
        private int waitTime = WAIT_TIME;

        private float alpha = 0f;

        private bool start = false;
        private bool done = false;

        private InputAction skip = InputAction.A;

        public Textbox(string text, TextChar character)
        {
            this.text = text;
            this.character = character;

            charName = getCharacterName(character);
            nameColor = getCharacterColour(character);
        }

        public void load(ContentManager Content)
        {
            nameFont = Content.Load<SpriteFont>("Fonts/MainFont");
            textFont = Content.Load<SpriteFont>("Fonts/MainFontSmall");

            portrait = Content.Load<Texture2D>("Sprites/" + getCharacterPortrait(character));

            // Calculate name and text position
            namePos = new Vector2(portrait.Width + PORTRAIT_OFF * 2, BaseGame.SCREEN_HEIGHT - portrait.Height - PORTRAIT_OFF + NAME_OFF);
            textPos = new Vector2(namePos.X, namePos.Y + TEXT_OFFSET);
        }

        public void setConversation(Conversation c)
        {
            conversation = c;
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (!start)
            {
                if (alpha < 1)
                {
                    alpha += FADE_RATE;
                }
                else
                {
                    alpha = 1;
                    start = true;
                }
            }

            if (!done && start)
            {
                // Update the text
                partText = text.Substring(0, (int)pos);

                if (partText.Length < text.Length)
                {
                    int endPos = partText.Length - 1;

                    if (partText[endPos] == '.' ||
                        partText[endPos] == ',' ||
                        partText[endPos] == '!' ||
                        partText[endPos] == '?' ||
                        partText[endPos] == '-')
                    {
                        pos += SCROLL_RATE / 10.0f;
                    }
                    else
                    {
                        pos += SCROLL_RATE;
                    }
                }
                else
                {
                    // Natural completion
                    done = true;
                }

                // Skipping text
                if (skip.Evaluate(inputState, null, out player))
                {
                    done = true;

                    partText = text;
                }
            }
            else if (done)
            {
                if (waitTime > 0)
                {
                    waitTime--;

                    // Skipping wait
                    if (skip.Evaluate(inputState, null, out player))
                    {
                        waitTime = 0;
                    }
                }
                else
                {
                    // Fade out
                    if (alpha > 0)
                    {
                        alpha -= FADE_RATE;
                    }
                    else
                    {
                        // Advance the conversation
                        conversation.nextTextbox();
                    }
                }
            }
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            sb.Begin();

            sb.Draw(portrait, new Vector2(PORTRAIT_OFF, BaseGame.SCREEN_HEIGHT - portrait.Height - PORTRAIT_OFF), Color.White * alpha);

            sb.DrawString(nameFont, charName, namePos, nameColor * alpha);
            sb.DrawString(textFont, partText, textPos, textColor * alpha);

            sb.End();
        }


        public static string getCharacterName(TextChar character)
        {
            switch(character)
            {
                case TextChar.LEO:
                {
                    return "Prince Leonard";
                }
                case TextChar.KLEPTO:
                {
                    return "Klepto";
                }
                case TextChar.CLEM:
                {
                    return "Clementine";
                }
                case TextChar.VILLAIN:
                {
                    return "???";
                }
                default:
                {
                    return "N/A";
                }
            }
        }

        public static string getCharacterPortrait(TextChar character)
        {
            switch (character)
            {
                case TextChar.LEO:
                {
                    return "Prince-Portrait";
                }
                case TextChar.KLEPTO:
                {
                    return "Thief-Portrait";
                }
                case TextChar.CLEM:
                {
                    return "Wolf-Portrait";
                }
                case TextChar.VILLAIN:
                {
                    return "Villain-Portrait";
                }
                default:
                {
                    return "Villain-Portrait";
                }
            }
        }

        public static Color getCharacterColour(TextChar character)
        {
            switch (character)
            {
                case TextChar.LEO:
                {
                    return Color.LightGoldenrodYellow;
                }
                case TextChar.KLEPTO:
                {
                    return Color.LightBlue;
                }
                case TextChar.CLEM:
                {
                    return Color.SandyBrown;
                }
                case TextChar.VILLAIN:
                {
                    return Color.Salmon;
                }
                default:
                {
                    return Color.White;
                }
            }
        }

        public override Vector3 getPOSITION()
        {
            throw new NotImplementedException();
        }
    }
}
