using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Titanium.Gambits
{
    class Quick : BaseGambit
    {
        public override void start(GameTime gameTime)
        {
            base.start(gameTime);
            finished = true;
        }
        public override void draw(Vector2 pos, SpriteBatch sb)
        {
        }

        public override void load(ContentManager content)
        {
        }

        public override int totalHeight()
        {
            return 1;
        }

        public override int totalWidth()
        {
            return 1;
        }
    }
}
