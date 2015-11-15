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
        InputAction rhythm;

        enum Directions { down, right, left, up }
        InputAction[] rhythmInputs = { 
            new InputAction(new Buttons[] {Buttons.A, Buttons.DPadDown }, new Keys[] { }, false),
            new InputAction(new Buttons[] {Buttons.B, Buttons.DPadRight }, new Keys[] { }, false),
            new InputAction(new Buttons[] {Buttons.X, Buttons.DPadLeft }, new Keys[] { }, false),
            new InputAction(new Buttons[] {Buttons.Y, Buttons.DPadUp }, new Keys[] { }, false)
        };

        MenuPanel mainMenu;

        bool inRhythm;
        InputAction held;
        int time;


        public delegate void MenuEventHandler(object sender, EventArgs e);

        public MainMenuScene(): base()
        {
            // Initialize the player actions
            arena = InputAction.A;
            rhythm = InputAction.START;
            battle = InputAction.X;

            // Create the actual Main Menu panel
            mainMenu = new MenuPanel("Main Menu", new List<MenuItem>()
            {
                new MenuItem("Rhtym Builder", rhythm),
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

            if (rhythm.wasPressed(inputState))
            {
                if (inRhythm)
                    printRhythmOutput();
                if (!inRhythm)
                    time = 0;
                inRhythm = !inRhythm;
            }

            if (inRhythm)
                updateRhythm(gameTime, inputState);
            else
            {
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
        public void updateRhythm(GameTime gameTime, InputState state)
        {
            this.time += gameTime.ElapsedGameTime.Milliseconds;
            if (held == null)
            {
                for (int i = 0; i < 4; i++)
                    if (rhythmInputs[i].wasPressed(state))
                    {
                        switch ((Directions)i)
                        {
                            case Directions.down:
                                Console.WriteLine("input: down");
                                break;
                            case Directions.left:
                                Console.WriteLine("LEFT WAS PRESSED :" + time);
                                break;
                            case Directions.right:
                                Console.WriteLine("RIGHT WAS PRESSED :" + time);
                                break;
                            case Directions.up:
                                Console.WriteLine("UP WAS PRESSED :" + time);
                                break;
                            default: break;
                        }
                        held = rhythmInputs[i];
                    }
            }
            else
            {
                if(!held.wasPressed(state) )
                {
                    held = null;
                    Console.WriteLine("RELEASED : " + time);
                }
            }
        }

        public void printRhythmOutput()
        {

        }

    }
}
