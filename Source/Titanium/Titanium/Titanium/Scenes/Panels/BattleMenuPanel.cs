using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Microsoft.Xna.Framework;
using Titanium.Gambits;
using Titanium.Utilities;
using Titanium.Battle;

namespace Titanium.Scenes.Panels
{
    /// <summary>
    /// Class contains a menu for each player controlled sprite.
    /// </summary>
    public class BattleMenuPanel: Panel
    {
        int height = 300;
        int width = 150;
        public int selected;
        Encounter encounter;

        public BattleMenuPanel(Encounter e)
        {
            selected = 0;
            encounter = e;
            foreach (PlayerSprite player in PartyUtils.getParty())
                addSubPanel(player.makeMenuPanel());
        }


        public override void center()
        {
            base.center();
            foreach (MenuPanel menu in subPanels)
                menu.updateMenuItemLocations();
        }

        public override void load(ContentManager content, Viewport v)
        {
            foreach (MenuPanel menu in subPanels)
                menu.load(content, v);

            base.load(content, v);
        }

        public override void draw(SpriteBatch sb)
        {
            switch(encounter.state)
            {
                case Encounter.EncounterState.HeroSelect:
                    break;
                case Encounter.EncounterState.ActionSelect:
                    subPanels.ElementAt(selected).draw(sb);
                    break;
                default:
                    break;
            }
            
            
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            base.update(gameTime, inputState);
        }

        public override float totalHeight()
        {
            return height;
        }

        public override float totalWidth()
        {
            return width;
        }
    }
}
