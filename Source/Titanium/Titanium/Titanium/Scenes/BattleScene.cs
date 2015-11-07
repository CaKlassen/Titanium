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
using System.Text;
using Titanium.Battle;
using Titanium.Utilities;

namespace Titanium.Scenes
{
    /**
     * This class provides a base for all game scenes.
     * 
     * Each game scene represents a distinct screen in the game.
     */
    class BattleScene : Scene
    {
        MenuPanel pauseMenu;

        bool paused;

        BaseGambit currentGambit;

        InputAction pause;
        InputAction arena;

        Encounter currentEncounter;

        /**
         * The default scene constructor.
         */
        public BattleScene(List<PartyUtils.Enemy> front, List<PartyUtils.Enemy> back) : base()
        {
            pause = new InputAction(
                new Buttons[] { Buttons.Start },
                new Keys[] { Keys.Escape },
                true
                );

            arena = new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true
                );

            pauseMenu = new MenuPanel("Pause Menu",
                new List<MenuItem>()
                {
                    new MenuItem("Back to arena", arena),
                    new MenuItem("Back to battle", pause)
                }
                );

            currentEncounter = new Encounter(front, back);
        }

        public BattleScene() : base()
        {
            pause = new InputAction(
                new Buttons[] { Buttons.Start },
                new Keys[] { Keys.Escape },
                true
                );

            arena = new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true
                );

            pauseMenu = new MenuPanel("Pause Menu",
                new List<MenuItem>()
                {
                    new MenuItem("Back to arena", arena),
                    new MenuItem("Back to battle", pause)
                }
                );

            currentEncounter = new Encounter(
                new List<Utilities.PartyUtils.Enemy>() { Utilities.PartyUtils.Enemy.Bat, Utilities.PartyUtils.Enemy.Bat },
                new List<Utilities.PartyUtils.Enemy>() { Utilities.PartyUtils.Enemy.Bat, Utilities.PartyUtils.Enemy.Bat }
                );
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {
            currentEncounter.load(content, SceneManager.GraphicsDevice.Viewport);
            pauseMenu.load(content, SceneManager.GraphicsDevice.Viewport);

            foreach (PlayerSprite player in PartyUtils.getParty())
                player.Load(content);
        }

        

        /**
         * The update function called in each frame.
         */
        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;
            if(paused)
            {
                if (pause.Evaluate(inputState, null, out player))
                    paused = false;
                if (arena.Evaluate(inputState, null, out player))
                    SceneManager.changeScene(SceneState.arena);
            }
            else if (pause.Evaluate(inputState, null, out player))
                paused = true;

            currentEncounter.update(gameTime, inputState);

            if (currentEncounter.success())
            {
                SceneManager.changeScene(SceneState.arena);
            }
            else if(currentEncounter.failure())
            {
                SceneManager.changeScene(SceneState.main);
            }
        }

        /**
         * The draw function called at the end of each frame.
         */
        public override void draw(GameTime gameTime)
        {
            SpriteBatch sb = SceneManager.SpriteBatch;

            sb.Begin();

            if (paused)
                pauseMenu.draw(sb);

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
