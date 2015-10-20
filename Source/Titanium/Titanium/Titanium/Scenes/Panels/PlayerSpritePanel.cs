using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Titanium.Gambits;

namespace Titanium.Scenes.Panels
{
    class PlayerSpritePanel: SpritePanel
    {
        enum Mode
        {
            target,
            input,
            gambit
        }

        Mode mode;

        BattleMenuPanel battleMenu;

        

        int selected;
        public int Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                resetStates();
            }
        }

        public PlayerSpritePanel(List<PlayerSprite> sprites, Side side): base(sprites.Cast<Sprite>().ToList(), side)
        {
            battleMenu = new BattleMenuPanel(sprites);
            selected = 0;
        }

        public override void load(ContentManager content)
        {
            battleMenu.load(content);
            foreach(PlayerSprite sprite in sprites)
            {
                sprite.state = PlayerSprite.UnitState.idle;
            }
            ((PlayerSprite)sprites[Selected]).state = PlayerSprite.UnitState.selected;

            base.load(content);
        }

        public override void draw(SpriteBatch sb)
        {
            battleMenu.draw(sb, Selected);
            base.draw(sb);
        }

        public bool selectNext()
        {            
            if (selected < sprites.Count - 1)
                Selected += 1;
            else
                Selected = 0;
            return finished();

        }

        public bool selectPrevious()
        {
            if (selected > 0)
                Selected -= 1;
            else
                Selected = sprites.Count - 1;
            return finished();
        }

        public void resetStates()
        {
            for (int i = 0 ; i < sprites.Count; i++)
            {
                if (((PlayerSprite)sprites[i]).state != PlayerSprite.UnitState.resting)
                {
                    ((PlayerSprite)sprites[i]).state = PlayerSprite.UnitState.idle;
                    if( i == Selected )
                        ((PlayerSprite)sprites[i]).state = PlayerSprite.UnitState.selected;
                }
            }
        }

        public Sprite.SpriteAction getAction(InputState inputState, out BaseGambit gambit)
        {
            gambit = null;
            InputAction action = battleMenu.getAction(inputState, Selected);
            if (action != null)
                return ((PlayerSprite)sprites[Selected]).getAction(action, out gambit);
            else
                return null;
        }

        public bool finished()
        {
            foreach (PlayerSprite sprite in sprites)
                if (sprite.state != PlayerSprite.UnitState.resting)
                    return false;

            return true;
        }

        public List<Sprite> Sprites()
        {
            return sprites;
        }

        public void activate()
        {
            foreach (PlayerSprite sprite in sprites)
                sprite.state = PlayerSprite.UnitState.idle;
            Selected = 0;
        }
    }
}
