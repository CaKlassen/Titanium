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
        static int topOffset = 50;
        static int leftOffset = 350;

        static string heroSelect = "Select a Hero";
        static string enemyTurn = "Enemy's turn";

        public int selected;
        Encounter encounter;
        SpriteFont font;
        Texture2D background;

        BaseGambit currentGambit;
        GambitResult result;

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

            font = content.Load<SpriteFont>("Fonts/TitleFont");
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

        public override void draw(SpriteBatch sb, Effect effect)
        

        {
			sb.Draw(background, Origin, Color.White);
            
            switch (encounter.state)
            {
                case Encounter.EncounterState.HeroSelect:
                    sb.DrawString(font, heroSelect, getCenter(heroSelect), Color.Black);
                    break;
                case Encounter.EncounterState.ActionSelect:
                    subPanels[selected].draw(sb, effect);
                    break;
                case Encounter.EncounterState.EnemyTurn:
                    sb.DrawString(font, enemyTurn, getCenter(enemyTurn), Color.Black);
                    if (encounter.enemyGambit())
                        currentGambit.draw(Position, sb);
                    break;
                case Encounter.EncounterState.Gambit:
                    currentGambit.draw(Position, sb);
                    break;
                default:
                    break;
            }

            
        }

        public void start(BaseGambit gambit, GameTime gameTime)
        {
            currentGambit = gambit;
            currentGambit.start(gameTime);
        }

        public bool gambitComplete(out GambitResult result)
        {
            if (currentGambit.isComplete(out result))
                return true;
            return false;
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            if (encounter.state == Encounter.EncounterState.Gambit || encounter.enemyGambit())
                currentGambit.update(gameTime, inputState);
        }

        public override float totalHeight()
        {
            return background.Height;
        }

        public override float totalWidth()
        {            
            return background.Width;
        }

        public Vector2 getCenter(string str)
        {
            Vector2 size = font.MeasureString(str);
            return new Vector2((background.Width / 2) - (size.X / 2), Position.Y);

        }
    }
}
