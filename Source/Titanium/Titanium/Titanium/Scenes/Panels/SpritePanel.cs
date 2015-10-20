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
        protected List<Sprite> sprites;
        Viewport? v;
        Side side;
        int height;

        List<Texture2D> icons;
        ContentManager content;


        public SpritePanel(List<Sprite> sprites, Side side):base()
        {
            this.sprites = sprites;
            this.side = side;
            v = null;
        }

        
        public override void load(ContentManager content)
        {
            foreach (Sprite sprite in sprites)
                sprite.Load(content);

            height = sprites[0].height();

            this.content = content;

            base.load(content);
        }

        public override void draw(SpriteBatch sb)
        {
            
            if (v == null)
            {
                v = sb.GraphicsDevice.Viewport;
                orientSprites(v.GetValueOrDefault());
            }

            if( icons != null )
            {
                for(int i=0; i<sprites.Count; ++i)
                {
                    sprites[i].Draw(sb);
                    sb.Draw(icons[i], side == Side.east ? sprites[i].getPosition() + new Vector2(50, 0) : sprites[i].getPosition() - new Vector2(50, 0), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
                }
            }
            
            else
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
                    Origin = new Vector2(200, 0);
                    break;
                case Side.east:
                default:
                    Origin = new Vector2(view.Width - sprites[0].width() - 200, 0 );
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

        public void target(bool target, List<InputAction> actions)
        {
            if (target)
            {
                this.icons = new List<Texture2D>();
                foreach (InputAction action in actions)
                    if (icons.Count < sprites.Count)
                        icons.Add(InputAction.GetIcon(content, action));
            }
            else
                this.icons = null;
        }

        public void act(List<Sprite> players)
        {
            foreach (Sprite sprite in sprites)
                sprite.quickAttack(players[0]);
        }

        public enum Side
        {
            east,
            west
        }

        

    }
}
