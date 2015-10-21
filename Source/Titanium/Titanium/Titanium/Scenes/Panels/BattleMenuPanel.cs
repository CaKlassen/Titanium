using System;  
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Microsoft.Xna.Framework;
using Titanium.Gambits;

namespace Titanium.Scenes.Panels
{
    /// <summary>
    /// Class contains a menu for each player controlled sprite.
    /// </summary>
    class BattleMenuPanel: Panel
    {

        Viewport? v;

        public BattleMenuPanel(List<PlayerSprite> heroes): base()
        {
            foreach(PlayerSprite hero in heroes)
            {
                addSubPanel(hero.getMenuPanel());
            }
            v = null;
        }



        public override void draw(SpriteBatch sb)
        {
            if( v == null )
            {
                int offset = 0;
                v = sb.GraphicsDevice.Viewport;
                Origin = new Vector2(0, v.GetValueOrDefault().Height - ((MenuPanel)subPanels[0]).totalHeight());
                foreach (MenuPanel menu in subPanels)
                {
                    menu.Origin = Origin;
                    menu.Offset = new Vector2(offset, 0);
                    menu.updateMenuItemLocations();
                    offset += (int)menu.totalWidth();
                }
            }

            base.draw(sb);
 
        }


        public void draw(SpriteBatch sb, int selected)
        {
            if (v == null)
            {
                int offset = 0;
                v = sb.GraphicsDevice.Viewport;
                Origin = new Vector2(0, v.GetValueOrDefault().Height - ((MenuPanel)subPanels[0]).totalHeight());
                foreach (MenuPanel menu in subPanels)
                {
                    menu.Origin = Origin;
                    menu.Offset = new Vector2(offset, 0);
                    menu.updateMenuItemLocations();
                    offset += (int)menu.totalWidth();
                }
            }

            subPanels[selected].draw(sb);

        }

        /// <summary>
        /// Get the action that the user wishes to perform
        /// </summary>
        /// <param name="inputState"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public InputAction getAction(InputState inputState, int selected)
        {
            MenuPanel panel = (MenuPanel)subPanels[selected];
            return panel.getSelectedAction(inputState);
        }
    }
}
