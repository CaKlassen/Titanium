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
    class Mash: BaseGambit
    {
        private int count = 0;

        private static InputAction action = new InputAction(new Buttons[] { Buttons.A }, new Keys[] { Keys.A }, true);

        private int timeLimit = 5000;
        private int timeLeft;

        private PlayerIndex controllingPlayer = PlayerIndex.One;

        SpriteFont font;

        public Mash(GameTime gameTime) : base(gameTime)
        {
            timeLeft = timeLimit;
        }

        public Mash() : base()
        {
            timeLeft = timeLimit;
        }

        public Mash(GameTime gameTime, int timeLimit) : base(gameTime)
        {
            this.timeLimit = timeLimit;
            this.timeLeft = timeLimit;
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");
        }

        public override void update(GameTime gameTime, InputState state)
        {
            PlayerIndex player;
            timeLeft = timeLimit - timeElapsed(gameTime);

            if (timeLeft <= 0)
            {
                finished = true;
            }
            else if (action.Evaluate(state, null, out player))
            {
                ++count;
            }
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

        public override bool isComplete(out float multiplier)
        {
            multiplier = 1f + ((count - 15)/100);
            return finished;
        }
    }
}
