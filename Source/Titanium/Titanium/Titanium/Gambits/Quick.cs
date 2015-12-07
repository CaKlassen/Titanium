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
        public Quick()
        {
            name = "Quick";
        }

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);

            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    multiplier = 0.3f;
                    break;
                case Difficulty.Medium:
                    multiplier = 0.2f;
                    break;
                case Difficulty.Hard:
                    multiplier = 0.1f;
                    break;
                default:
                    multiplier = 0.2f;
                    break;

            }
            
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
