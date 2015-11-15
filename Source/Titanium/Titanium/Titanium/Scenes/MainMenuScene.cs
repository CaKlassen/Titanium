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

namespace Titanium.Scenes
{
    /// <summary>
    /// The Main Menu
    /// </summary>
    class MainMenuScene : Scene
    {
        ContentManager content;

        // Possible player actions
        InputAction arena;
        InputAction battle;

        private static int MOVE_SPEED = 15;

        private Texture2D menuBackground;
        private Texture2D menuTitle;

        private Vector2 titlePos;

        MenuPanel mainMenu;

        public delegate void MenuEventHandler(object sender, EventArgs e);

        public MainMenuScene(): base()
        {
            // Initialize the player actions
            arena = InputAction.A;

            battle = InputAction.X;

            // Create the actual Main Menu panel
            mainMenu = new MenuPanel("Main Menu", new List<MenuItem>()
            {
                new MenuItem("New Game", arena),
                new MenuItem("Load Game", arena),
                new MenuItem("(TEMP) Battle", battle)
            });

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

            mainMenu.load(content, SceneManager.GraphicsDevice.Viewport);

            mainMenu.center();
        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (arena.Evaluate(inputState, null, out player))
            {
                SceneManager.changeScene(SceneState.arena);
#if XBOX360
                SaveUtils.getInstance().RegisterStorage();
#endif
            }
            else if (battle.Evaluate(inputState, null, out player))
            {
                BattleScene battle = new BattleScene(
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Redbat, PartyUtils.Enemy.Redbat },
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Redbat, PartyUtils.Enemy.Redbat }
                    );
                SceneManager.setScene(SceneState.battle, battle, true);
            }
            mainMenu.update(gameTime, inputState);

            // Move the title image
            titlePos.X += MathUtils.smoothChange(titlePos.X, 0, MOVE_SPEED);
        }
    }
}
