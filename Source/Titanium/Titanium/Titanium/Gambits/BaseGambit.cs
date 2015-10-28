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
    /// <summary>
    /// Base class representing the actions the player must take to improve a certain attack or spell
    /// </summary>
    abstract class BaseGambit
    {
        protected double startTime;
        protected bool finished;
        protected float multiplier = 1f;
        protected Viewport? v = null;
        protected Vector2 position = Vector2.Zero;
        protected SpriteFont font;

        public BaseGambit(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            finished = false;
        }

        public BaseGambit() { finished = false; }

        /// <summary>
        /// Start this gambit from the given time. Must be called before the gambit can be used
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        public virtual void start(GameTime gameTime) { startTime = gameTime.TotalGameTime.TotalMilliseconds; }

        /// <summary>
        /// The time since the gambit was started
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <returns></returns>
        public int timeElapsed(GameTime gameTime)
        {
            return (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime);
        }

        /// <summary>
        /// Returns true if this gambit is complete. The multiplier for this gambit is passed through the out variable.
        /// </summary>
        /// <param name="multiplier">Float that will be assigned the multiplier based on the user's performance</param>
        /// <returns></returns>
        public virtual bool isComplete(out float multiplier)
        {
            multiplier = this.multiplier;
            return finished;
        }

        public abstract void load(ContentManager content);

        public abstract void update(GameTime gameTime, InputState state);

        public abstract void draw(SpriteBatch sb);
        
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
    }
}
