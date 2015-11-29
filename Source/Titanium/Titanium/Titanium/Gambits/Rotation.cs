﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Utilities;

namespace Titanium.Gambits
{
    class Rotation : BaseGambit
    {
        // The time limit in ms
        int timeLimit = 6000;

        // The current input the user must perform
        int current;

        // The time InputAction.RSLEFT before the gambit is finished
        int timeleft;

        Texture2D[] icons;

        Texture2D icon;

        bool? clockwise;

        PlayerIndex player;

        int count;
        int maxRotations = 15;

        float multStep;

        InputAction[] circle;

        public Rotation() : base()
        {
            icons = new Texture2D[2];
            name = "Rotation";
            message = "Rotate the stick in the direction shown!";
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
            sb.DrawString(font, msg, pos + new Vector2(0, icon.Height), Color.Black);
            base.draw(pos, sb);
        }

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    maxRotations = 10;
                    timeLimit = 5000;
                    break;
                case Difficulty.Medium:
                    maxRotations = 15;
                    timeLimit = 5750;
                    break;
                case Difficulty.Hard:
                    maxRotations = 20;
                    timeLimit = 6500;
                    break;
                default:
                    maxRotations = 15;
                    timeLimit = 5750;
                    break;

            }
            current = 0;
            count = 0;
            multStep = 1f / maxRotations;
            if(clockwise == null)
                clockwise = new Random(gameTime.TotalGameTime.Milliseconds).Next(2) == 0;

            icon = icons[clockwise.GetValueOrDefault() ? 0 : 1];
            timeleft = timeLimit;
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/NumbersFont");

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
                SoundUtils.Play(SoundUtils.Sound.Success);
                ++count;
                current = 0;
            }

            if (timeleft < 0)
            {
                finished = true;
                SoundUtils.Play(SoundUtils.Sound.Complete);
                multiplier = 0f + (count * multStep);
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
