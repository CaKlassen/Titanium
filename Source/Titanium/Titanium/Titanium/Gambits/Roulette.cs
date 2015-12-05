using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Utilities;

namespace Titanium.Gambits
{
    class Roulette: BaseGambit
    {
        Texture2D[] icons;
        public static string[] iconDirs = { "arrow-up", "arrow-left", "arrow-down", "arrow-right" };

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/NumbersFont");

            List<Texture2D> icons = new List<Texture2D>();
            foreach (string dir in iconDirs)
                icons.Add(content.Load<Texture2D>("ButtonIcons/" + dir));

            this.icons = icons.ToArray();

        }
        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
        }

        public override int totalHeight()
        {
            throw new NotImplementedException();
        }

        public override int totalWidth()
        {
            throw new NotImplementedException();
        }

        class RouletteWheel
        {
            static int width = 100;
            static int height = 300;
            static int iconWidth = 50;
            static int iconHeight = 50;


            bool spinning;
            int icon;
            Texture2D[] icons;
            Texture2D pixel;

            Rectangle leftLine, rightLine;
            Rectangle topBuffer, bottomBuffer;

            

            public RouletteWheel(Texture2D[] icons)
            {
                this.icons = icons;
                leftLine = new Rectangle();
                rightLine= new Rectangle();
                topBuffer = new Rectangle();
                bottomBuffer = new Rectangle();
            }

            public void draw(SpriteBatch sb, Vector2 pos)
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    pixel.SetData(new Color[] { Color.White });
                }

            }

        }
    }
}
