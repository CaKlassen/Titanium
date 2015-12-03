using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Utilities;

namespace Titanium.Gambits
{
    
    class Finesse: BaseGambit
    {

        static InputAction[] leftIndex = {
            new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { Keys.J },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { Keys.K },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { Keys.J },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { Keys.K },
                false
                ),
        };
        static InputAction[] rightIndex = {
            new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { Keys.I },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { Keys.L },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { Keys.I },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { Keys.L },
                false
                ),
        };
        static InputAction[] leftThumb = {
            new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Z },
                false
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadDown , Buttons.LeftThumbstickDown},
                new Keys[] { Keys.X },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.DPadLeft , Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.Z },
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.DPadRight , Buttons.LeftThumbstickRight },
                new Keys[] { Keys.X },
                false
                )
        };
        InputAction[] rightThumb = {
            new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.V},
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.C},
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.V},
                false
                ),
            new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.C},
                false
                )
        };

        static int leftOffset = 100;
        static int topOffset = 100;

        List<InputAction[]> actionString;
        int current = 0;
        int timeLeft;
        int timeLimit = 8000;
        int length = 3;
        float multStep;
        Random rng;

        public Finesse()
        {
            name = "Finesse";
            message = "Press the buttons together!";
            helpOffset += new Vector2(200, 0);
        }

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    length = 3;
                    timeLimit = 8000;
                    break;
                case Difficulty.Medium:
                    length = 4;
                    timeLimit = 9000;
                    break;
                case Difficulty.Hard:
                    length = 5;
                    timeLimit = 10000;
                    break;
                default:
                    length = 4;
                    timeLimit = 9000;
                    break;

            }
            multStep = 1f / length;
            rng = new Random(gameTime.TotalGameTime.Milliseconds);
            actionString = makeActionString(length);
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
            font = content.Load<SpriteFont>("Fonts/NumbersFontBig");
        }

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            drawCurrentAction(pos, sb);
            sb.DrawString(font, "Time Remaining: " + timeString(timeLeft), pos + new Vector2(200, 50), Color.Black);
            base.draw(pos, sb);
        }

        public override void update(GameTime gameTime, InputState state)
        {
            base.update(gameTime, state);
            timeLeft = timeLimit - timeElapsed;
            if (timeLeft <= 0)
            {
                SoundUtils.Play(SoundUtils.Sound.Failure);
                multiplier = current * multStep;
                finished = true;
            }
            if(current >= actionString.Count)
            {
                SoundUtils.Play(SoundUtils.Sound.Complete);
                multiplier = 1f;
                finished = true;
            }
            else if(actionPressed(actionString[current], state))
            {
                SoundUtils.Play(SoundUtils.Sound.Success);
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
