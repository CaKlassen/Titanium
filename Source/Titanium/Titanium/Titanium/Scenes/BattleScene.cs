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
        public BattleScene(Encounter encounter) : base()
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


            currentEncounter = encounter;
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {
            currentEncounter.load(content);
            pauseMenu.load(content);
            pauseMenu.center(SceneManager.GraphicsDevice.Viewport);
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
