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

namespace Titanium.Scenes
{
    /// <summary>
    /// The Main Menu
    /// </summary>
    class MainMenuScene : Scene
    {
        List<Panel> panels;
        ContentManager content;
        SpriteFont font;

        // Possible player actions
        InputAction arena;
        InputAction battle;

        MenuPanel mainMenu;

        public delegate void MenuEventHandler(object sender, EventArgs e);

        public MainMenuScene(): base()
        {
            panels = new List<Panel>();

            // Initialize the player actions
            arena = InputAction.A;

            battle = InputAction.X;

            // Create the actual Main Menu panel
            mainMenu = new MenuPanel("Main Menu", new List<MenuItem>()
            {
                new MenuItem("Enter the arena!", arena),
                new MenuItem("Enter battle!", battle)
            });


            panels.Add(mainMenu);
        }

        public override void draw(GameTime gameTime)
        {
            SceneManager.SpriteBatch.Begin();
            foreach (Panel panel in panels)
                panel.draw(SceneManager.SpriteBatch);
            SceneManager.SpriteBatch.End();
        }

        // 
        public override void loadScene(ContentManager content)
        {
            font = content.Load<SpriteFont>("TestFont");

            foreach (Panel panel in panels)
                panel.load(content);

            mainMenu.center(SceneManager.GraphicsDevice.Viewport);
        }

        public override void unloadScene() {}

        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            if (arena.Evaluate(inputState, null, out player))
                SceneManager.changeScene(SceneState.arena);
            else if (battle.Evaluate(inputState, null, out player))
                SceneManager.changeScene(SceneState.battle);
        }
    }
}
