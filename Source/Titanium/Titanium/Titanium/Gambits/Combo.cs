using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Titanium.Gambits
{
    class Combo : BaseGambit
    {

        static InputAction[] buttons = {
            new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.A },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.B },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { Keys.X },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { Keys.D1 },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { Keys.D0 },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.DPadUp },
                new Keys[] { Keys.Up },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.DPadDown },
                new Keys[] { Keys.Down },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.DPadLeft },
                new Keys[] { Keys.Left },
                true
            ),
            new InputAction(
                new Buttons[] { Buttons.DPadRight },
                new Keys[] { Keys.Right },
                true
            )
        };

        int length = 4;
        int timeLimit = 10000;

        int current;
        int timeLeft;

        List<InputAction> comboString;
        List<Texture2D> icons;

        SpriteFont font;

        public Combo(GameTime gameTime):base(gameTime)
        {
            comboString = makeComboString(gameTime);
            icons = new List<Texture2D>();
            current = 0;
        }

        public override void draw(SpriteBatch sb)
        {
            int width = 0;
            int height = 0;
            Texture2D icon;

            for (int i=0; i<icons.Count; i++)
            {
                icon = icons[i];
                sb.Draw(icon, new Vector2(width, height), i < current ? Color.Black : Color.White);
                width += icon.Width;
            }
            height += icons[0].Height;
            string msg = "Time Left: " + TimeSpan.FromMilliseconds(timeLeft);
            sb.DrawString(font, msg, new Vector2(0, height), Color.Red);
        }

        public override void load(ContentManager content)
        {
            foreach (InputAction action in comboString)
                icons.Add(InputAction.GetIcon(content, action));

            font = content.Load<SpriteFont>("TestFont");
                
        }

        public override void update(GameTime gameTime, InputState state)
        {
            PlayerIndex player;
            timeLeft = timeLimit - timeElapsed(gameTime);

            if (timeLeft <= 0)
            {
                multiplier = 0.75f;
                finished = true;
            }
            if (comboString.ElementAt(current).Evaluate(state, null, out player))
            {
                current++;
                if(current >= comboString.Count)
                {
                    multiplier = 0.75f + (((float)(timeLimit - timeElapsed(gameTime))/100)*5);
                    finished = true;
                }
            }
            else if (miss(state, player))
            {
                multiplier = 0.75f + (((float)current / 100) * 2);
                finished = true;
            }
        }

        public bool miss(InputState state, PlayerIndex player)
        {
            foreach (InputAction button in buttons)
            {
                if (!button.Evaluate(state, null, out player))
                    continue;
                else
                    return true;
            }
            return false;
        }

        public List<InputAction> makeComboString(GameTime gameTime)
        {
            List<InputAction> combo = new List<InputAction>();
            int inputCount = buttons.Length-1;
            Random r = new Random(gameTime.TotalGameTime.Seconds);

            for (int i = 0; i < length; i++)
                combo.Add(buttons.ElementAt(r.Next(inputCount)));

            return combo;
        }
    }
}
