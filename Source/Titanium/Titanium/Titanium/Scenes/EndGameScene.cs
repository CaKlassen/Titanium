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
using Titanium.Entities;

namespace Titanium.Scenes
{
    /// <summary>
    /// The Main Menu
    /// </summary>
    class EndGameScene : Scene
    {
        ContentManager content;

        private SpriteFont titleFont;
        private SpriteFont scoreFont;

        // Possible player actions
        InputAction goBack;

        private static int MOVE_SPEED = 15;

        private Texture2D menuBackground;
        private Vector2 scoreTitlePos;
        private Vector2 scorePos;
        private string scoreTitleString = "FINAL SCORE";
        private string scoreString;
        private Conversation currentConversation = null;

        public delegate void MenuEventHandler(object sender, EventArgs e);

        public EndGameScene(bool win) : base()
        {
            if (win)
            {
                currentConversation = DialogueUtils.makeConversation(ConversationType.END_WIN);
            }
            else
            {
                currentConversation = DialogueUtils.makeConversation(ConversationType.END_LOSE);
            }

            // Initialize the player actions
            goBack = InputAction.A;

            // Create the score string
            scoreString = "" + ArenaController.instance.getScore();

            scorePos = new Vector2(BaseGame.SCREEN_WIDTH / 2 + 230, BaseGame.SCREEN_HEIGHT);
            scoreTitlePos = new Vector2(BaseGame.SCREEN_WIDTH / 2 + 230, -64);
        }

        public override void draw(GameTime gameTime)
        {
            SceneManager.SpriteBatch.Begin();

            SceneManager.SpriteBatch.Draw(menuBackground, new Vector2(0, 0), Color.White);

            SceneManager.SpriteBatch.DrawString(titleFont, scoreTitleString, scoreTitlePos, Color.White);
            SceneManager.SpriteBatch.DrawString(scoreFont, scoreString, scorePos, Color.White);

            SceneManager.SpriteBatch.End();

            if (currentConversation != null)
            {
                currentConversation.Draw(SceneManager.SpriteBatch, null);
            }
        }

        // 
        public override void loadScene(ContentManager content)
        { 
            // Load the art
            menuBackground = content.Load<Texture2D>("Sprites/Menu-Background");

            // Load the font
            titleFont = content.Load<SpriteFont>("Fonts/TitleFont");
            scoreFont = content.Load<SpriteFont>("Fonts/NumbersFontBig");

            // Adjust the score location
            scorePos.X += titleFont.MeasureString(scoreTitleString).X / 2 - scoreFont.MeasureString(scoreString).X / 2;

            currentConversation.load(content);
        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (currentConversation != null)
            {
                currentConversation.Update(gameTime, inputState);

                // Check if we are done talking
                if (currentConversation.getDone())
                {
                    currentConversation = null;
                }
            }

            if (currentConversation == null)
            {
                if (goBack.Evaluate(inputState, null, out player))
                {
                    SaveUtils save = SaveUtils.getInstance();
                    save.DeleteSaveFile();
                    PartyUtils.Reset();

                    // Return to the main menu
                    SceneManager.changeScene(SceneState.main);
                }

                // Update the score position
                scorePos.Y += MathUtils.smoothChange(scorePos.Y, 200, MOVE_SPEED);
                scoreTitlePos.Y += MathUtils.smoothChange(scoreTitlePos.Y, 120, MOVE_SPEED);
            }
        }
    }
}
