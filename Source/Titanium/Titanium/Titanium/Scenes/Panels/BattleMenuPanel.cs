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
    public class BattleMenuPanel: MenuPanel
    {

        Viewport? v;

        public BattleMenuPanel(): base("Battle Menu")
        {
            
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

    }
}
