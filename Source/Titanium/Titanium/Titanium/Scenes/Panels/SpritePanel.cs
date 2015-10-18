using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Scenes.Panels
{
    class SpritePanel: Panel
    {
        List<Sprite> sprites;
        Viewport? v;
        Side side;
        int height;

        public SpritePanel(List<Sprite> sprites, Side side):base()
        {
            this.sprites = sprites;
            this.side = side;
            v = null;
        }

        public SpritePanel(List<PlayerSprite> sprites, Side side)
        {
            this.side = side;
            v = null;
            this.sprites = sprites.Cast<Sprite>().ToList();
        }

        public override void load(ContentManager content)
        {
            foreach (Sprite sprite in sprites)
                sprite.Load(content);

            height = sprites[0].height();

            base.load(content);
        }

        public override void draw(SpriteBatch sb)
        {
            
            if (v == null)
            {
                v = sb.GraphicsDevice.Viewport;
                orientSprites(v.GetValueOrDefault());
            }

            foreach (Sprite sprite in sprites)
                sprite.Draw(sb);

            base.draw(sb);
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            
            foreach (Sprite sprite in sprites)
                sprite.Update(gameTime, inputState);

            base.update(gameTime, inputState);
        }

        public void orientSprites(Viewport view)
        {
            int currentHeight = 0;
            switch(side)
            {
                case Side.west:
                    Origin = Vector2.Zero;
                    break;
                case Side.east:
                default:
                    Origin = new Vector2(view.Width - sprites[0].width(), 0 );
                    break;
            }
            foreach(Sprite sprite in sprites)
            {
                sprite.move((int)Origin.X, currentHeight);
                currentHeight += height;
            }
        }

        public int count()
        {
            return sprites.Count;
        }

        public Sprite at(int i)
        {
            if (i < sprites.Count)
                return sprites[i];
            return null;
        }

        public enum Side
        {
            east,
            west
        }
    }
}
