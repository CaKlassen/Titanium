using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Titanium.Gambits
{
    
    class Finesse: BaseGambit
    {

        static InputAction[] leftIndex = {
            new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { },
                false
                ),
        };
        static InputAction[] rightIndex = {
            new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { },
                false
                ),
        };
        static InputAction[] leftThumb = {
            new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new Keys[] { },
                false
                ),
        };
        InputAction[] rightThumb = {
            new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { },
                false
                ),
        };

        static int leftOffset = 100;
        static int topOffset = 100;

        List<InputAction[]> actionString;
        int current = 0;
        int timeLeft;
        int timeLimit = 7000;
        SpriteFont font;
        Random rng;

        public override void start(GameTime gameTime)
        {
            rng = new Random(gameTime.TotalGameTime.Milliseconds);
            base.start(gameTime);
            actionString = makeActionString(4);
            current = 0;
            timeLeft = timeLimit;
        }

        public List<InputAction[]> makeActionString(int length)
        {
            List<InputAction[]> result = new List<InputAction[]>(length);

            for(int i=0; i< length; ++i)
            {
                result.Add(makeAction());
            }
            return result;
        }

        public InputAction[] makeAction()
        {
            InputAction[] action = new InputAction[]
            {
                leftIndex[rng.Next(leftIndex.Count())],
                rightIndex[rng.Next(rightIndex.Count())],
                leftThumb[rng.Next(leftThumb.Count())],
                rightThumb[rng.Next(rightThumb.Count())],
            };
            return action;
        }

        public bool actionPressed(InputAction[] actions, InputState state)
        {
            foreach(InputAction action in actions)
            {
                if (!action.wasPressed(state))
                    return false;
            }
            return true;
        }

        public void drawCurrentAction(Vector2 pos, SpriteBatch sb)
        {
            if (current >= actionString.Count)
                return;
            InputAction[] action = actionString[current];
            sb.Draw(action[0].icon(), pos, Color.White);
            sb.Draw(action[1].icon(), pos + new Vector2(leftOffset, 0), Color.White);
            sb.Draw(action[2].icon(), pos + new Vector2(0, topOffset), Color.White);
            sb.Draw(action[3].icon(), pos + new Vector2(leftOffset, topOffset), Color.White);
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");
        }

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            drawCurrentAction(pos, sb);
            sb.DrawString(font, "Time Left: " + TimeSpan.FromMilliseconds(timeLeft), pos + new Vector2(100, 0), Color.Red);
        }

        public override void update(GameTime gameTime, InputState state)
        {
            base.update(gameTime, state);
            timeLeft = timeLimit - timeElapsed;
            if (timeLeft <= 0)
            {
                multiplier = 0.7f + (current/10);
                finished = true;
            }
            if(current >= actionString.Count)
            {
                multiplier = 1.2f;
                finished = true;
            }
            else if(actionPressed(actionString[current], state))
            {
                current++;
            }

        }

        public override int totalWidth()
        {
            throw new NotImplementedException();
        }

        public override int totalHeight()
        {
            throw new NotImplementedException();
        }
    }
}
