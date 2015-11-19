using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Titanium.Utilities;

namespace Titanium.Gambits
{
    /// <summary>
    /// Gambit that has the user perform a series of inputs correctly and within a certain time frame
    /// </summary>
    public class Combo : BaseGambit
    {
        

        // The possible buttons in the combo
        static InputAction[] buttons = {
            InputAction.A,
            InputAction.B,
            InputAction.X,
            InputAction.Y,
            InputAction.LB,
            InputAction.RB,
            InputAction.LT,
            InputAction.RT,
            InputAction.UP,
            InputAction.DOWN,
            InputAction.LEFT,
            InputAction.RIGHT,
        };

        // The number of inputs the user must perform
        int length = 8;

        // The time limit in ms
        int timeLimit = 7000;

        // The current input the user must perform
        int current;

        // The time left before the gambit is finished
        int timeLeft;

        // The actual combo buttons to be input
        List<InputAction> comboString;

        // The icon associated with each combo action
        List<Texture2D> icons;

        float multStep;

        public Combo()
        {
            name = "Combo";
            message = "Press the buttons in order!";
        }

        public override void start(GameTime gameTime)
        {
            base.start(gameTime);
            icons = new List<Texture2D>();
            current = 0;
            comboString = makeComboString(DateTime.Now.Millisecond);
            timeLeft = timeLimit;
            multStep = 1 / 8f;
        }

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            int width = 0;
            int height = 0;
            Texture2D icon;

            for (int i=0; i<icons.Count; i++)
            {
                icon = icons[i];
                sb.Draw(icon, pos + new Vector2(width, height), i < current ? Color.Black : Color.White);
                width += icon.Width;
            }
            height += icons[0].Height;
            sb.DrawString(font, "Time Remaining: " + timeString(timeLeft), pos + new Vector2(0, height), Color.Black);
            base.draw(pos, sb);
        }

        public override void load(ContentManager content)
        {

            font = content.Load<SpriteFont>("Fonts/NumbersFontBig");

        }

        public override void update(GameTime gameTime, InputState state)
        {

            base.update(gameTime, state);
            timeLeft = timeLimit - timeElapsed;

            if (timeElapsed < 5)
                return;

            if (timeLeft <= 0)
            {
                SoundUtils.Play(SoundUtils.Sound.Failure);
                multiplier = 0 + (current * multStep);
                finished = true;
            }
            if (comboString[current].wasPressed(state))
            {
                SoundUtils.Play(SoundUtils.Sound.Success);
                current++;
                if(current >= comboString.Count)
                {
                    SoundUtils.Play(SoundUtils.Sound.Complete);
                    multiplier = 1f;
                    finished = true;
                }
            }
            else if (miss(state))
            {
                SoundUtils.Play(SoundUtils.Sound.Failure);
                current = 0;
            }
        }

        /// <summary>
        /// Detects if the player input the wrong button
        /// </summary>
        /// <param name="state">The input state to check</param>
        /// <param name="player">The controlling player</param>
        /// <returns>true if the user missed, false if they didnt</returns>
        public bool miss(InputState state)
        {
            foreach (InputAction button in buttons)
            {
                if (!button.wasPressed(state))
                    continue;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Makes a random combo string with the given seed.
        /// </summary>
        /// <param name="seed">The seed for the random number generator</param>
        /// <returns>The combo string</returns>
        public List<InputAction> makeComboString(int seed)
        {
            List<InputAction> combo = new List<InputAction>();
            int inputCount = buttons.Length-1;
            Random r = new Random(seed);

            for (int i = 0; i < length; i++)
                combo.Add(buttons.ElementAt(r.Next(inputCount)));

            foreach (InputAction action in combo)
                icons.Add(action.icon());

            return combo;
        }

        public override int totalWidth()
        {
            int w = 0;
            foreach (Texture2D icon in icons)
                w += icon.Width;
            return w;
        }

        public override int totalHeight()
        {
            int h = 0;
            foreach (Texture2D icon in icons)
                h = icon.Height > h ? icon.Height : h;
            return h + font.LineSpacing;
        }

        

    }
}
