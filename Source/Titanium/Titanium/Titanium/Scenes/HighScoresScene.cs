using System;
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
    class HighScoresScene : Scene
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
        private string scoreTitleString = "HIGH SCORES";
        private string scoreString;

        public delegate void MenuEventHandler(object sender, EventArgs e);

        public HighScoresScene(List<int> highScores) : base()
        {
            // Initialize the player actions
            goBack = InputAction.B;

            // Create the score string
            scoreString = "";

            foreach (int score in highScores)
            {
                scoreString += score + "\n";
            }

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
        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (goBack.Evaluate(inputState, null, out player))
            {
                // Return to the main menu
                SceneManager.changeScene(SceneState.main);
            }

            // Update the score position
            scorePos.Y += MathUtils.smoothChange(scorePos.Y, 200, MOVE_SPEED);
            scoreTitlePos.Y += MathUtils.smoothChange(scoreTitlePos.Y, 120, MOVE_SPEED);
        }
    }
}
