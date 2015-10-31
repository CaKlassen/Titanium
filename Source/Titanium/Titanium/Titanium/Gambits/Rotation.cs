using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Titanium.Gambits
{
    class Rotation : BaseGambit
    {
        // The time limit in ms
        int timeLimit = 5000;

        // The current input the user must perform
        int current;

        // The time left before the gambit is finished
        int timeLeft;

        Texture2D[] icons;

        Texture2D icon;

        bool? clockwise;

        static InputAction left, right, up, down;

        PlayerIndex player;

        int count;

        static Rotation()
        {
            left = new InputAction(
                new Buttons[] {Buttons.RightThumbstickLeft},
                new Keys[] { Keys.Left },
                true
                );
            right = new InputAction(
                new Buttons[] { Buttons.RightThumbstickRight },
                new Keys[] { Keys.Right },
                true
                );
            up = new InputAction(
                new Buttons[] { Buttons.RightThumbstickUp },
                new Keys[] { Keys.Up },
                true
                );
            down = new InputAction(
                new Buttons[] { Buttons.RightThumbstickDown },
                new Keys[] { Keys.Down },
                true
                );
        }

        InputAction[] circle;

        public Rotation() : base()
        {
            icons = new Texture2D[2];
        }

        public Rotation(int timeLimit) : base()
        {
            icons = new Texture2D[2];
            this.timeLimit = timeLimit;
        }

        public Rotation(bool clockwise) : base()
        {
            icons = new Texture2D[2];
        }

        public Rotation(int timeLimit, bool clockwise) : base()
        {
            icons = new Texture2D[2];
            this.timeLimit = timeLimit;
            this.clockwise = clockwise;
        }

        public override void draw(SpriteBatch sb)
        {
            if (v == null)
            {
                v = sb.GraphicsDevice.Viewport;
                position = new Vector2((v.GetValueOrDefault().Width / 2) - (totalWidth() / 2), (v.GetValueOrDefault().Height / 2) - (totalHeight() / 2));
            }
            sb.Draw(icon, position, Color.White);
            string msg = "Time Left: " + TimeSpan.FromMilliseconds(timeLeft) + "\nCount: " + count;
            sb.DrawString(font, msg, position + new Vector2(0, icon.Height), Color.Red);
        }

        public override void start(GameTime gameTime)
        {
            current = 0;
            count = 0;

            if(clockwise == null)
                clockwise = new Random(gameTime.TotalGameTime.Milliseconds).Next(2) == 0;

            icon = icons[clockwise.GetValueOrDefault() ? 0 : 1];
            timeLeft = timeLimit;
            base.start(gameTime);
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");

            icons[0] = content.Load<Texture2D>("ButtonIcons/HUD-Stick-Right-CW");
            icons[1] = content.Load<Texture2D>("ButtonIcons/HUD-Stick-Right-CCW");
        }

        public override int totalHeight()
        {
            return icon.Height;
        }

        public override int totalWidth()
        {
            return icon.Width;
        }

        public override void update(GameTime gameTime, InputState state)
        {
            timeLeft = timeLimit - timeElapsed(gameTime);

            if (circle == null)
            {
                if (left.Evaluate(state, null, out player))
                    makeCircle(Direction.left);
                if (right.Evaluate(state, null, out player))
                    makeCircle(Direction.right);
                if (up.Evaluate(state, null, out player))
                    makeCircle(Direction.up);
                if (down.Evaluate(state, null, out player))
                    makeCircle(Direction.down);
            }
            else if (current < circle.Length)
            {
                if (circle[current].Evaluate(state, null, out player))
                    ++current;
            }
            else
            {
                ++count;
                current = 0;
            }

            if (timeLeft < 0)
            {
                finished = true;
                multiplier = 0.7f + ((count - 5) / 10);
            }
        }

        private void makeCircle(Direction start)
        {
            List<InputAction> actions;
            switch(start)
            {
                case Direction.left:
                    actions = new List<InputAction>() { left, up, right, down };
                    break;
                case Direction.right:
                    actions = new List<InputAction>() { right, down, left, up };
                    break;
                case Direction.up:
                    actions = new List<InputAction>() { up, right, down, left };
                    break;
                case Direction.down:
                default:
                    actions = new List<InputAction>() { down, left, up, right };
                    break;
            }

            if (!clockwise.GetValueOrDefault())
                actions.Reverse();

            circle = actions.ToArray();

            current = 1;
        }

        enum Direction
        {
            left,
            right,
            up,
            down
        }
    }
}
