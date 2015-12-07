using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Content;
using Titanium.Utilities;

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
        private static InputAction[] actions = { InputAction.A, InputAction.B, InputAction.X, InputAction.Y };

        string msg = "MASH ";

        // The time limit in ms
        private int timeLimit = 5000;
        private int timeLeft;

        PlayerIndex player;
        InputAction action;
        Random rng;


        int ceiling = 25;

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    ceiling = 25;
                    timeLimit = 5000;
                    break;
                case Difficulty.Medium:
                    ceiling = 35;
                    timeLimit = 6500;
                    break;
                case Difficulty.Hard:
                    ceiling = 45;
                    timeLimit = 8000;
                    break;
                default:
                    ceiling = 25;
                    timeLimit = 5000;
                    break;

            }
            rng = new Random(gameTime.TotalGameTime.Seconds);
            action = actions[rng.Next(actions.Length)];
            count = 0;
            timeLeft = timeLimit;
        }

        public Mash() 
        {
            name = "Mash";
            message = "Hit the button as fast as possible!";
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/NumbersFontBig");
        }

        public override void update(GameTime gameTime, InputState state)
        {
            
            timeLeft = timeLimit - timeElapsed;

            if (timeLeft <= 0)
            {
                SoundUtils.Play(SoundUtils.Sound.Complete);
                finished = true;
                multiplier = ((float) count / ceiling);
            }
            else if (action.Evaluate(state, null, out player))
            {
                SoundUtils.Play(SoundUtils.Sound.Input);
                ++count;
            }
            base.update(gameTime, state);
        }

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            sb.DrawString(font, count.ToString(), pos + new Vector2(80, 15), Color.Black);
            sb.Draw(action.icon(), pos, Color.White);
            
            sb.DrawString(font, "Time Remaining: " + timeString(timeLeft), pos + new Vector2(0, 60), Color.Black);
            base.draw(pos, sb);
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
