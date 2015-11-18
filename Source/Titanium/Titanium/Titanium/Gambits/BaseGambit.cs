using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Scenes;

namespace Titanium.Gambits
{
    public struct GambitResult
    {
        public GambitResult(float multiplier, int completionTime)
        {
            this.multiplier = multiplier;
            this.completionTime = completionTime;
        }
        public float multiplier;
        public int completionTime;
    }
    /// <summary>
    /// Base class representing the actions the player must take to improve a certain attack or spell
    /// </summary>
    public abstract class BaseGambit
    {
        protected int startTime;
        protected int timeElapsed;
        protected bool finished;
        GambitResult result;
        protected SpriteFont font;
        protected float multiplier = 0f;
        protected string name;
        protected string message = "Help text goes here";
        protected Vector2 helpOffset = new Vector2(0, 120);
        public BaseGambit(GameTime gameTime)
        {
            startTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            finished = false;
        }

        public BaseGambit() { finished = false; }

        /// <summary>
        /// Start this gambit from the given time. Must be called before the gambit can be used
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        public virtual void start(GameTime gameTime)
        {
            startTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            timeElapsed = 0;
            result = new GambitResult(multiplier, int.MaxValue);
            finished = false;
        }

        /// <summary>
        /// Returns true if this gambit is complete. The multiplier for this gambit is passed through the out variable.
        /// </summary>
        /// <param name="multiplier">Float that will be assigned the multiplier based on the user's performance</param>
        /// <returns></returns>
        public virtual bool isComplete(out GambitResult result)
        {
            if (finished)
            {
                result = new GambitResult(multiplier, timeElapsed);
                finished = false;
                return true;
            }
            result = this.result;
            return false;
        }

        public abstract void load(ContentManager content);

        public virtual void update(GameTime gameTime, InputState state)
        {
            timeElapsed = (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime);
        }

        public virtual void draw(Vector2 pos, SpriteBatch sb) { sb.DrawString(font, message, pos + helpOffset, Color.Blue); }
        
        /// <summary>
        /// Returns the total width in pixels of this gambit
        /// </summary>
        /// <returns>The total width</returns>
        public abstract int totalWidth();

        /// <summary>
        /// returns the total height in pixels of this gambit
        /// </summary>
        /// <returns>The total height</returns>
        public abstract int totalHeight();

        public override string ToString()
        {
            return name;
        }

        protected string timeString(int timeLeft)
        {
            return "  " + TimeSpan.FromMilliseconds(timeLeft).Seconds.ToString() + "." + TimeSpan.FromMilliseconds(timeLeft).Milliseconds.ToString();
        }
    }
}
