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
        InputAction menu;

        Encounter currentEncounter;

        Texture2D background;
        Rectangle screen;

        private Conversation currentConversation = null;
        private bool initialize = true;


        /**
         * The default scene constructor.
         */
        public BattleScene(List<PartyUtils.Enemy> front, List<PartyUtils.Enemy> back, Conversation conversation) : base()
        {
            menu = InputAction.Y;
            pause = InputAction.START;

            pauseMenu = new MenuPanel("Pause Menu",
                new List<MenuItem>()
                {
                    new MenuItem("Resume Game", pause),
                    new MenuItem("Main Menu", menu)
                }
                );

            bgm = SoundUtils.Music.BattleTheme;

            currentEncounter = new Encounter(front, back);

            currentConversation = conversation;
        }

        public BattleScene() : base()
        {
            menu = InputAction.Y;
            pause = InputAction.START;

            pauseMenu = new MenuPanel("Pause Menu",
                new List<MenuItem>()
                {
                    new MenuItem("Resume Game", pause),
                    new MenuItem("Main menu", menu)
                }
                );

            currentEncounter = new Encounter(
                new List<Utilities.PartyUtils.Enemy>() { Utilities.PartyUtils.Enemy.Bat, Utilities.PartyUtils.Enemy.Bat },
                new List<Utilities.PartyUtils.Enemy>() { Utilities.PartyUtils.Enemy.Bat, Utilities.PartyUtils.Enemy.Bat }
                );
            bgm = SoundUtils.Music.BattleTheme;
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {
            screen = new Rectangle(0, 0, SceneManager.GraphicsDevice.Viewport.Width, SceneManager.GraphicsDevice.Viewport.Height);
            currentEncounter.load(content, SceneManager.GraphicsDevice.Viewport);
            pauseMenu.load(content, SceneManager.GraphicsDevice.Viewport);
            background = content.Load<Texture2D>("Sprites/Battle-Base");
            foreach (PlayerSprite player in PartyUtils.getParty())
                player.Load(content);

            background = content.Load<Texture2D>("Sprites/Battle-Base");
            pauseMenu.center();

            if (currentConversation != null)
            {
                currentConversation.load(content);
            }
            
        }

        

        /**
         * The update function called in each frame.
         */
        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (InputAction.START.wasPressed(inputState))
            {
                SoundUtils.Play(SoundUtils.Sound.Input);
                paused = !paused;
            }

            if (currentConversation != null)
            {
                currentConversation.Update(gameTime, inputState);

                // Check if we are done talking
                if (currentConversation.getDone())
                {
                    currentConversation = null;
                }
            }

            if (paused)
            {
                if (menu.Evaluate(inputState, null, out player))
                {
                    SoundUtils.Play(SoundUtils.Sound.Input);
                    SceneManager.changeScene(SceneState.main);
                }
            }
            else
            {
                if (currentConversation == null || initialize)
                {
                    initialize = false;

                    currentEncounter.update(gameTime, inputState);

                    if (currentEncounter.success())
                    {
                        SceneManager.changeScene(SceneState.arena);
                    }
                    else if (currentEncounter.failure())
                    {
                        // Save the player's achieved score
                        SaveUtils save = SaveUtils.getInstance();
                        HighscoreData data = save.loadHighScores();
                        HighScoreUtils.updateHighScores(data.highscores, ArenaController.instance.getScore());
                        save.saveHighScores(data.highscores);

                        ArenaScene.instance.SceneManager.setScene(SceneState.endGame, new EndGameScene(false), true);
                    }
                }
            }
            
        }

        /**
         * The draw function called at the end of each frame.
         */
        public override void draw(GameTime gameTime)
        {
            SpriteBatch sb = SceneManager.SpriteBatch;

            sb.Begin();

            sb.Draw(background, screen, Color.White);

            currentEncounter.draw(sb, null);
            if (paused)
                pauseMenu.draw(sb, null);

            sb.End();

            // Draw the conversation
            if (currentConversation != null)
            {
                currentConversation.Draw(sb, null);
            }
        }

        /**
         * This function is called when a scene is no longer active.
         */
        public override void unloadScene()
        {

        }

    }
}
