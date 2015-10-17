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
    abstract class BaseGambit
    {
        protected double startTime;
        protected bool finished;
        protected float multiplier = 1f;

        public BaseGambit(GameTime gameTime)
        {
            startTime = gameTime.TotalGameTime.TotalMilliseconds;
            finished = false;
        }

        public int timeElapsed(GameTime gameTime)
        {
            return (int)(gameTime.TotalGameTime.TotalMilliseconds - startTime);
        }

        public bool isComplete(out float multiplier)
        {
            multiplier = this.multiplier;
            return finished;
        }

        public abstract void load(ContentManager content);

        public abstract void update(GameTime gameTime, InputState state);

        public abstract void draw(SpriteBatch sb);
    }
}
