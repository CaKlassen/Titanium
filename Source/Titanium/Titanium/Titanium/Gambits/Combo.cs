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

namespace Titanium.Gambits
{
    /// <summary>
    /// Gambit that has the user perform a series of inputs correctly and within a certain time frame
    /// </summary>
    public class Combo : BaseGambit
    {
        // Sound Effects
        SoundEffect sfxSuccess;
        SoundEffect sfxFailure;
        SoundEffect sfxComplete;

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
        int timeLimit = 10000;

        // The current input the user must perform
        int current;

        // The time left before the gambit is finished
        int timeLeft;

        // The actual combo buttons to be input
        List<InputAction> comboString;

        // The icon associated with each combo action
        List<Texture2D> icons;

        public override void start(GameTime gameTime)
        {
            icons = new List<Texture2D>();
            current = 0;
            comboString = makeComboString(DateTime.Now.Millisecond);
            timeLeft = timeLimit;
            base.start(gameTime);
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
            string msg = "Time Left: " + TimeSpan.FromMilliseconds(timeLeft);
            sb.DrawString(font, msg, pos + new Vector2(0, height), Color.Red);
        }

        public override void load(ContentManager content)
        {

            font = content.Load<SpriteFont>("TestFont");

            sfxComplete = content.Load<SoundEffect>("sfx/complete");
            sfxFailure = content.Load<SoundEffect>("sfx/failure");
            sfxSuccess = content.Load<SoundEffect>("sfx/success");

        }

        public override void update(GameTime gameTime, InputState state)
        {

            base.update(gameTime, state);
            timeLeft = timeLimit - timeElapsed;

            if (timeElapsed < 5)
                return;

            if (timeLeft <= 0)
            {
                multiplier = 0.7f;
                finished = true;
                sfxFailure.Play();
            }
            if (comboString[current].wasPressed(state))
            {
                current++;
                sfxSuccess.Play(0.2f, 0f, 0f);
                if(current >= comboString.Count)
                {
                    multiplier = 1.2f;
                    finished = true;
                    sfxComplete.Play(0.4f, 0f, 0f);
                }
            }
            else if (miss(state))
            {
                current = 0;
                sfxFailure.Play();
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
                icons.Add(InputAction.GetIcon(action));

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
