using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Gambits
{
    /// <summary>
    /// Represents the gambit that has the player repeatedly press a button in a certain time frame;
    /// </summary>
    public class Mash: BaseGambit
    {
        // The number of times the user has pressed the button
        private int count = 0;

        // The button the user will have to mash
        private static InputAction action = InputAction.A;

        // The time limit in ms
        private int timeLimit = 5000;
        private int timeLeft;

        PlayerIndex player;


        public Mash() : base()
        {
            timeLeft = timeLimit;
        }

        public Mash(int timeLimit) 
        {
            this.timeLimit = timeLimit;
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");
        }

        public override void update(GameTime gameTime, InputState state)
        {
            
            timeLeft = timeLimit - timeElapsed;

            if (timeLeft <= 0)
            {
                finished = true;
                multiplier = 0.7f + ((count - 15) / 10);
            }
            else if (action.Evaluate(state, null, out player))
            {
                ++count;
            }
            base.update(gameTime, state);
        }

        public override void draw(SpriteBatch sb)
        {
            if (v == null)
            {
                v = sb.GraphicsDevice.Viewport;
                position = new Vector2((v.GetValueOrDefault().Width / 2) - (totalWidth() / 2), (v.GetValueOrDefault().Height / 2) - (totalHeight() / 2));
            }

            string msg = "Times Pressed: " + count + "\nTime Left: " + TimeSpan.FromMilliseconds(timeLeft);

            sb.DrawString(font, msg, position, Color.Red);
        }

        public override int totalHeight()
        {
            return font.LineSpacing * 2;
        }

        public override int totalWidth()
        {
            return (int)font.MeasureString("Times Pressed: " + count + "\nTime Left: " + TimeSpan.FromMilliseconds(timeLeft)).X;
        }

    }
}
