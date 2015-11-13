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

        // The time InputAction.RSLEFT before the gambit is finished
        int timeleft;

        Texture2D[] icons;

        Texture2D icon;

        bool? clockwise;

        PlayerIndex player;

        int count;

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

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            sb.Draw(icon, pos, Color.White);
            string msg = "Time left: " + TimeSpan.FromMilliseconds(timeleft) + "\nCount: " + count;
            sb.DrawString(font, msg, pos + new Vector2(0, icon.Height), Color.Red);
        }

        public override void start(GameTime gameTime)
        {
            current = 0;
            count = 0;

            if(clockwise == null)
                clockwise = new Random(gameTime.TotalGameTime.Milliseconds).Next(2) == 0;

            icon = icons[clockwise.GetValueOrDefault() ? 0 : 1];
            timeleft = timeLimit;
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
            timeleft = timeLimit - timeElapsed;

            if (circle == null)
            {
                if (InputAction.RSLEFT.wasPressed(state))
                    makeCircle(Direction.left);
                if (InputAction.RSRIGHT.Evaluate(state, null, out player))
                    makeCircle(Direction.right);
                if (InputAction.RSUP.Evaluate(state, null, out player))
                    makeCircle(Direction.up);
                if (InputAction.RSDOWN.Evaluate(state, null, out player))
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

            if (timeleft < 0)
            {
                finished = true;
                multiplier = 0.7f + ((count - 5) / 10);
            }
            base.update(gameTime, state);
        }

        private void makeCircle(Direction start)
        {
            List<InputAction> actions;
            switch(start)
            {
                case Direction.left:
                    actions = new List<InputAction>() { InputAction.RSLEFT, InputAction.RSUP, InputAction.RSRIGHT, InputAction.RSDOWN };
                    break;
                case Direction.right:
                    actions = new List<InputAction>() { InputAction.RSRIGHT, InputAction.RSDOWN, InputAction.RSLEFT, InputAction.RSUP };
                    break;
                case Direction.up:
                    actions = new List<InputAction>() { InputAction.RSUP, InputAction.RSRIGHT, InputAction.RSDOWN, InputAction.RSLEFT };
                    break;
                case Direction.down:
                default:
                    actions = new List<InputAction>() { InputAction.RSDOWN, InputAction.RSLEFT, InputAction.RSUP, InputAction.RSRIGHT };
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
