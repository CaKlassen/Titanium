using FileHelpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Scenes.Panels;

namespace Titanium.Battle
{
    class Encounter
    {
        SpritePanel heroes;
        SpritePanel enemies;

        static List<InputAction> targetActions;

        BattleMenuPanel battleMenu;
        static Encounter()
        {
            targetActions = new List<InputAction>()
            {
                new InputAction(
                    new Buttons[] { Buttons.A },
                    new Keys[] { Keys.A },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.B },
                    new Keys[] { Keys.B },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.X },
                    new Keys[] { Keys.X },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.Y },
                    new Keys[] { Keys.Y },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.LeftShoulder },
                    new Keys[] { Keys.D1 },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.RightShoulder },
                    new Keys[] { Keys.D0 },
                    true
                )
            };
        }

        public Encounter(List<PlayerSprite> heroes, List<Sprite> enemies)
        {
            this.heroes = new SpritePanel(heroes, SpritePanel.Side.west);
            this.heroes = new SpritePanel(heroes, SpritePanel.Side.east);
            battleMenu = new BattleMenuPanel(heroes);
        }

        public Encounter()
        {
            /************************************************
            Sprite Creation Area; to be done via file parsing
            ************************************************/
            List<PlayerSprite> heroList = new List<PlayerSprite>()
            {
                new PlayerSprite(this),
                new PlayerSprite(this)
            };
            loadStats(heroList.Cast<Sprite>().ToList(), "PlayerFile.txt");
            heroes = new SpritePanel(heroList, SpritePanel.Side.east);

            List<Sprite> enemyList = new List<Sprite>
            {
                new Sprite(),
                new Sprite()
            };
            loadStats(enemyList, "Stage_1_1.txt");
            enemies = new SpritePanel(enemyList, SpritePanel.Side.west);

            battleMenu = new BattleMenuPanel(heroList);
        }

        public void loadStats(List<Sprite> l, String target)
        {
            var engine = new FileHelperAsyncEngine<UnitStats>();
            String path = "../../../../TitaniumContent/Stats/";
            using (engine.BeginReadFile(path + target))
            {
                List<UnitStats> tempList = new List<UnitStats>();
                foreach (UnitStats u in engine)
                {
                    tempList.Add(u);
                }

                for (int i = 0; i < l.Count; ++i)
                    l[i].setParam(tempList[i], (int)Vector2.Zero.X, (int)Vector2.Zero.Y);
            }
        }

        public void load(ContentManager content)
        {
            heroes.load(content);
            enemies.load(content);
            battleMenu.load(content);
        }

        public void draw(SpriteBatch sb)
        {
            heroes.draw(sb);
            enemies.draw(sb);
            battleMenu.draw(sb);
        }

        public void update(GameTime gameTime, InputState inputState)
        {
            heroes.update(gameTime, inputState);
            enemies.update(gameTime, inputState);
            battleMenu.update(gameTime, inputState);
        }

        public Sprite targetSelected(InputState inputState)
        { 
            PlayerIndex player;
            for(int i=0; i<enemies.count(); ++i)
            {
                if (targetActions[i].Evaluate(inputState, null, out player))
                    return enemies.at(i);
            }
            return null;
        }

        
    }
}
