﻿using System;
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
        static int topOffset = 50;
        static int leftOffset = 350;

        public int selected;
        Encounter encounter;
        SpriteFont font;
        Texture2D background;

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

            font = content.Load<SpriteFont>("TestFont");
            background = content.Load<Texture2D>("Sprites/Battle-HUD");

            Offset = new Vector2(leftOffset, topOffset);
            updateMenuLocations();
            

            base.load(content, v);
        }

        public void updateMenuLocations()
        {
            foreach (MenuPanel menu in subPanels)
            {
                menu.Origin = this.Position;
                menu.updateMenuItemLocations();
            }
        }

        public override void draw(SpriteBatch sb)
        {
            sb.Draw(background, Origin, Color.White);
            switch (encounter.state)
            {
                case Encounter.EncounterState.HeroSelect:
                    sb.DrawString(font, "Select a hero", Position, Color.Black);
                    break;
                case Encounter.EncounterState.ActionSelect:
                    subPanels.ElementAt(selected).draw(sb);
                    break;
                case Encounter.EncounterState.EnemyTurn:
                    sb.DrawString(font, "Enemy turn", Position, Color.Black);
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
            return background.Height;
        }

        public override float totalWidth()
        {
            return background.Width;
        }
    }
}
