﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Scenes.Panels;
using Titanium.Battle;
using Titanium.Utilities;
using Titanium.Gambits;

namespace Titanium.Scenes
{
    /// <summary>
    /// The Main Menu
    /// </summary>
    class MainMenuScene : Scene
    {
        ContentManager content;        

        // Possible player actions
        InputAction newGame;
        InputAction loadGame;
        InputAction scores;

        private static int MOVE_SPEED = 15;

        private Texture2D menuBackground;
        private Texture2D menuTitle;

        private Vector2 titlePos;
        private Vector2 menuPos;

        private SaveUtils save;
        MenuPanel mainMenu;
       
        public delegate void MenuEventHandler(object sender, EventArgs e);

        public MainMenuScene(): base()
        {
            // Initialize the player actions
            newGame = InputAction.A;
            loadGame = InputAction.X;
            scores = InputAction.Y;

            // Create the actual Main Menu panel
            mainMenu = new MenuPanel("Main Menu", new List<MenuItem>()
            {
                new MenuItem("New Game", newGame),
                new MenuItem("Load Game", loadGame),
                new MenuItem("High Scores", scores)
            });

            // Create a high scores file if it doesn't exist
            save = SaveUtils.getInstance();
        }

        public override void draw(GameTime gameTime)
        {
            SceneManager.SpriteBatch.Begin();

            SceneManager.SpriteBatch.Draw(menuBackground, new Vector2(0, 0), Color.White);
            SceneManager.SpriteBatch.Draw(menuTitle, titlePos, Color.White);
            mainMenu.draw(SceneManager.SpriteBatch, null);
            SceneManager.SpriteBatch.End();
        }

        // 
        public override void loadScene(ContentManager content)
        { 
            // Load the art
            menuBackground = content.Load<Texture2D>("Sprites/Menu-Background");
            menuTitle = content.Load<Texture2D>("Sprites/Menu-Title");

            titlePos = new Vector2(-menuTitle.Width, 0);
            menuPos = new Vector2(300, 0);

            mainMenu.load(content, SceneManager.GraphicsDevice.Viewport);
            mainMenu.center();

#if XBOX360
            if (!save.storageRegistered())
                save.RegisterStorage();
#endif
            if (!save.CheckHighScoreExists())
            {
                List<int> templateScore = HighScoreUtils.createInitialHighScores();
                save.saveHighScores(templateScore);
            }

        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (newGame.Evaluate(inputState, null, out player))
            {
                SoundUtils.Play(SoundUtils.Sound.Input);
                menuNewGame();
            }
            else if (loadGame.Evaluate(inputState, null, out player))
            {
                SoundUtils.Play(SoundUtils.Sound.Input);
                if (save.CheckFileExists())
                {
                    menuLoadGame();
                }
            }
            else if (scores.Evaluate(inputState, null, out player))
            {
                SoundUtils.Play(SoundUtils.Sound.Input);
                menuHighScores();
            }

            mainMenu.update(gameTime, inputState);

            // Move the title image
            titlePos.X += MathUtils.smoothChange(titlePos.X, 0, MOVE_SPEED);
            
            // Move the menu
            menuPos.X += MathUtils.smoothChange(menuPos.X, -380, MOVE_SPEED);

            mainMenu.Origin = new Vector2(BaseGame.SCREEN_WIDTH + menuPos.X, 340);
            mainMenu.updateMenuItemLocations();
        }

        public void menuNewGame()
        {

            ArenaScene arena = new ArenaScene();
            PartyUtils.Reset();
            SceneManager.setScene(SceneState.arena, arena, true);
        }

        public void menuLoadGame()
        {
            SaveData data = SaveUtils.getInstance().loadGame();
            ArenaScene arena = new ArenaScene(data);
            SceneManager.setScene(SceneState.arena, arena, true);
        }

        public void menuBattle()
        {
            BattleScene battle = new BattleScene(
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Spider, PartyUtils.Enemy.Spider },
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Spider, PartyUtils.Enemy.Spider },
                    null
                    );
            SceneManager.setScene(SceneState.battle, battle, true);
        }

        public void menuHighScores()
        {
            HighscoreData data = save.loadHighScores();

            HighScoresScene score = new HighScoresScene(data.highscores);
            SceneManager.setScene(SceneState.highScores, score, true);
        }
    }
}
