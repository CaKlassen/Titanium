using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Titanium.Entities;
using Titanium.Gambits;
using Titanium.Scenes.Panels;
using FileHelpers;
using System.Text;

namespace Titanium.Scenes
{
    /**
     * This class provides a base for all game scenes.
     * 
     * Each game scene represents a distinct screen in the game.
     */
    class BattleScene : Scene
    {
        List<Panel> panels;
        MenuPanel battleMenu;

        private List<Sprite> AllySprites = new List<Sprite>();
        private List<Sprite> EnemySprites = new List<Sprite>();

        private enum State
        {
            character,
            target,
            gambit
        }

        State currentState;

        BaseGambit currentGambit;

        InputAction mash;
        InputAction menu;
        InputAction arena;
        InputAction combo;

        /**
         * The default scene constructor.
         */
        public BattleScene() : base()
        {
            currentState = State.character;

            mash = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.Enter },
                true
                );
            combo = new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true
                );
            menu = new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.Back, Keys.Escape },
                true
                );

            arena = new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { Keys.Space },
                true
                );

            panels = new List<Panel>();

            battleMenu = new MenuPanel(Vector2.Zero, "Battle Options");
            List<MenuItem> options = new List<MenuItem>()
            {
                new MenuItem("Mash!", mash, battleMenu),
                new MenuItem("Combo!", combo, battleMenu),
                new MenuItem("Back to Arena", arena, battleMenu),
                new MenuItem("Back to Main Menu", menu, battleMenu)
            };
            panels.Add(battleMenu);
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {



            // Draw the UI Components
            foreach (Panel panel in panels)
                panel.load(content);

            /************************************************
            Sprite Creation Area; to be done via file parsing
            ************************************************/
            Sprite s = new Sprite();
            AllySprites.Add(s);
            Sprite t = new Sprite();
            AllySprites.Add(t);

            loadStats(AllySprites, "PlayerFile.txt", 0);

            Sprite u = new Sprite();
            EnemySprites.Add(u);
            Sprite v = new Sprite();
            EnemySprites.Add(v);

            loadStats(EnemySprites, "Stage_1_1.txt", 1);

            foreach (Sprite sp in AllySprites)
            {
                sp.Load(content);
            }
            foreach (Sprite sp in EnemySprites)
            {
                sp.Load(content);
            }
            /***********************************************
            Ends Here
            ************************************************/
            battleMenu.center(SceneManager.GraphicsDevice.Viewport);

        }

        public void loadStats(List<Sprite> l, String target, int side)
        {
            var engine = new FileHelperAsyncEngine<UnitStats>();
            int xpos = 0;
            if (side == 0)
                xpos = 1000;
            else if (side == 1)
                xpos = 500;
            int ypos = 150;
            String path = "../../../../TitaniumContent/Stats/";

            using (engine.BeginReadFile(path + target))
            {
                List<UnitStats> tempList = new List<UnitStats>();
                foreach (UnitStats u in engine)
                {
                    tempList.Add(u);
                }

                for (int i = 0; i < l.Count; ++i)
                {
                    l[i].setParam(tempList[i], xpos, ypos);
                    ypos += 150;
                }
            }
        }

        /**
         * The update function called in each frame.
         */
        public override void update(GameTime gameTime, InputState inputState)
        {
            float multiplier;

            PlayerIndex player;
            if (currentState == State.gambit)
            {
                if (currentGambit.isComplete(out multiplier))
                {
                    currentState = State.character;
                }
                else
                    currentGambit.update(gameTime, inputState);
            }
            else if (mash.Evaluate(inputState, null, out player))
            {
                currentState = State.gambit;
                currentGambit = new Mash(gameTime, mash);
                currentGambit.load(SceneManager.Game.Content);
            }
            else if (arena.Evaluate(inputState, null, out player))
            {
                SceneManager.changeScene(SceneState.arena);
            }
            else if (menu.Evaluate(inputState, null, out player))
            {
                SceneManager.changeScene(SceneState.main);
            }
            else if (combo.Evaluate(inputState, null, out player))
            {
                currentState = State.gambit;
                currentGambit = new Combo(gameTime);
                currentGambit.load(SceneManager.Game.Content);
            }
            foreach (Sprite sp in AllySprites)
            {
                sp.Update(gameTime);
            }
            foreach (Sprite sp in EnemySprites)
            {
                sp.Update(gameTime);
            }
        }

        /**
         * The draw function called at the end of each frame.
         */
        public override void draw(GameTime gameTime)
        {
            SpriteBatch sb = SceneManager.SpriteBatch;

            sb.Begin();

            foreach (Sprite sp in AllySprites)
            {
                sp.Draw(sb);
            }
            foreach (Sprite sp in EnemySprites)
            {
                sp.Draw(sb);
            }

            if (currentState == State.gambit)
            {
                currentGambit.draw(sb);
            }

            foreach (Panel panel in panels)
                panel.draw(sb);

            sb.End();
        }

        /**
         * This function is called when a scene is no longer active.
         */
        public override void unloadScene()
        {

        }

    }
}
