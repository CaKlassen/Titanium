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
        SpriteFont font;

        // Possible player actions
        InputAction arena;
        InputAction battle;

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
                new MenuItem("Enter the arena!", arena),
                new MenuItem("Enter battle!", battle)
            });

        }

        public override void draw(GameTime gameTime)
        {
            SceneManager.SpriteBatch.Begin();
            mainMenu.draw(SceneManager.SpriteBatch, null);
            SceneManager.SpriteBatch.End();
        }

        // 
        public override void loadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");
            mainMenu.load(content, SceneManager.GraphicsDevice.Viewport);

            mainMenu.center();
        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (arena.Evaluate(inputState, null, out player))
                SceneManager.changeScene(SceneState.arena);
            else if (battle.Evaluate(inputState, null, out player))
            {
                BattleScene battle = new BattleScene(
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Redbat, PartyUtils.Enemy.Redbat },
                    new List<PartyUtils.Enemy>() { PartyUtils.Enemy.Redbat, PartyUtils.Enemy.Redbat }
                    );
                SceneManager.setScene(SceneState.battle, battle, true);
            }
            mainMenu.update(gameTime, inputState);
        }
    }
}
