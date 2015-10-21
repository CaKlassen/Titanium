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
    /// <summary>
    /// Class represents the Player controlled characters on the battlefield
    /// </summary>
    class PlayerSpritePanel: SpritePanel
    {
        // The menu to be used to control the units
        BattleMenuPanel battleMenu;
        
        // The currently selected unit
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

        /// <summary>
        /// Select the next unit
        /// </summary>
        /// <returns>Whether or not all units have acted</returns>
        public bool selectNext()
        {            
            if (selected < sprites.Count - 1)
                Selected += 1;
            else
                Selected = 0;
            return finished();

        }

        /// <summary>
        /// Select the previous unit
        /// </summary>
        /// <returns>Whether or not all units have acted</returns>
        public bool selectPrevious()
        {
            if (selected > 0)
                Selected -= 1;
            else
                Selected = sprites.Count - 1;
            return finished();
        }

        /// <summary>
        /// Reset the states of the player controlled units, unless they have already acted.
        /// </summary>
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

        /// <summary>
        /// Get the SpriteAction delegate associated with the user's inputs
        /// </summary>
        /// <param name="inputState">The state of the inputs</param>
        /// <param name="gambit">The gambit the player must perform, if there is one</param>
        /// <returns>The SpriteAction delegate associated with the user's inputs</returns>
        public Sprite.SpriteAction getAction(InputState inputState, out BaseGambit gambit)
        {
            gambit = null;
            InputAction action = battleMenu.getAction(inputState, Selected);
            if (action != null)
                return ((PlayerSprite)sprites[Selected]).getAction(action, out gambit);
            else
                return null;
        }

        /// <summary>
        /// Returns true if all units have acted this turn
        /// </summary>
        /// <returns></returns>
        public bool finished()
        {
            foreach (PlayerSprite sprite in sprites)
                if (sprite.state != PlayerSprite.UnitState.resting)
                    return false;

            return true;
        }

        /// <summary>
        /// Returns the list of sprites
        /// </summary>
        /// <returns></returns>
        public List<Sprite> Sprites()
        {
            return sprites;
        }

        /// <summary>
        /// Activate all player controlled sprites in prep for a new turn.
        /// </summary>
        public void activate()
        {
            foreach (PlayerSprite sprite in sprites)
                sprite.state = PlayerSprite.UnitState.idle;
            Selected = 0;
        }
    }
}
