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
using Titanium.Battle;

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

        Encounter currentEncounter;

        /**
         * The default scene constructor.
         */
        public BattleScene(Encounter encounter) : base()
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
            

            currentEncounter = encounter;
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {

            currentEncounter.load(content);

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
                currentGambit = new Mash(gameTime);
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

            currentEncounter.update(gameTime, inputState);
        }

        /**
         * The draw function called at the end of each frame.
         */
        public override void draw(GameTime gameTime)
        {
            SpriteBatch sb = SceneManager.SpriteBatch;

            sb.Begin();

            currentEncounter.draw(sb);

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
