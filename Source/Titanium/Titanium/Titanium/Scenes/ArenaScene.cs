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
using Titanium.Arena;
using Titanium.Utilities;
using Titanium.Battle;
using Titanium.Entities.Traps;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using Titanium.Scenes.Panels;

namespace Titanium.Scenes
{
    /**
     * This class provides a base for all game scenes.
     * 
     * Each game scene represents a distinct screen in the game.
     */
    class ArenaScene : Scene
    {
        public static ArenaScene instance;

        private Tile[,] baseArena;

        private Tile StartTile;

        SaveData GameSave;

        private bool loaded;

        // Possible user actions
        InputAction menu,
                    battle,
                    rotateUp,
                    rotateDown,
                    pause;

        ContentManager content;
        public Effect HLSLeffect;

        public ArenaController controller;
        public Character Hero;
        public Camera camera;
        private ArenaTable table;
        private ArenaSkybox skybox;
        private BasicEffect effect;
        public int potionsUsed;

        private float FlashLightAngle;
        private bool flashOn = false;
        private int FLASH_RATE = 10;
        private int flashTimer = 60;
        private float MIN_FLASH = 0f;
        private float MAX_FLASH = 35f;

        public static float ARENA_AMBIENCE = 0.080f;
        private static Color ambientColour = new Color(0.318f, 0.365f, 0.404f, 1);

        bool paused = false;
        MenuPanel pauseMenu;

        public List<Entity> collidables;

        /**
         * The default scene constructor.
         */
        public ArenaScene() : base()
        {
            instance = this;
            loaded = false;

            // Create the arena controller
            controller = new ArenaController();

            // Define the user actions
            menu = InputAction.Y;
            pause = InputAction.START;

            rotateDown = new InputAction(
                new Buttons[] { Buttons.RightThumbstickDown },
                new Keys[] { Keys.NumPad2 },
                false
                );
            rotateUp = new InputAction(
                new Buttons[] { Buttons.RightThumbstickUp },
                new Keys[] { Keys.NumPad8 },
                false
                );

            bgm = SoundUtils.Music.ArenaTheme;
            pauseMenu = new MenuPanel("Pause Menu", new List<MenuItem>() {
                new MenuItem("Back to Arena", pause),
                new MenuItem("Main Menu", menu)
            });
        }

        public ArenaScene(SaveData data) : base()
        {
            instance = this;
            loaded = true;

            // Create the arena controller
            controller = new ArenaController(data);

            // Set the player hp
            List<PlayerSprite> party = PartyUtils.getParty();
            party[0].setHealth(data.partyHealth[0]);
            party[1].setHealth(data.partyHealth[1]);
            party[2].setHealth(data.partyHealth[2]);

            // Define the user actions
            menu = InputAction.Y;
            pause = InputAction.START;

            rotateDown = new InputAction(
                new Buttons[] { Buttons.RightThumbstickDown },
                new Keys[] { Keys.NumPad2 },
                false
                );
            rotateUp = new InputAction(
                new Buttons[] { Buttons.RightThumbstickUp },
                new Keys[] { Keys.NumPad8 },
                false
                );

            bgm = SoundUtils.Music.ArenaTheme;
            pauseMenu = new MenuPanel("Pause Menu", new List<MenuItem>() {
                new MenuItem("Back to Arena", pause),
                new MenuItem("Main Menu", menu)
            });
        }

        /**
         * This function is called when a scene is made active.
         */
        public override void loadScene(ContentManager content)
        {
            collidables = new List<Entity>();

            if (content == null)
                content = new ContentManager(SceneManager.Game.Services, "Content");

            // Load the shader
            HLSLeffect = content.Load<Effect>("Effects/Shader");

            if (!loaded)
            {
                GameSave.seed = Environment.TickCount;

                GameSave.level = controller.getLevel();
                GameSave.partyHealth = PartyUtils.getPartyHealth();
                GameSave.score = controller.getScore();
                //SAVE
                SaveUtils.getInstance().saveGame(GameSave);

                controller.setGenerator(new Random(GameSave.seed));
            }

            // Generate the arena
            ArenaBuilder builder = new ArenaBuilder(controller.getLevelSize(controller.getLevel()), controller.getLevelSize(controller.getLevel()), 
                content, SceneManager.GraphicsDevice.Viewport.AspectRatio, controller.getLevelDifficulty(controller.getLevel()));
            baseArena = builder.buildArenaBase();
            StartTile = builder.getStartTile();

            //
            if (effect == null)
                effect = new BasicEffect(SceneManager.Game.GraphicsDevice);//null

            FlashLightAngle = MIN_FLASH;
            Hero = new Character();
            camera = new Camera(effect, SceneManager.Game.Window.ClientBounds.Width, SceneManager.Game.Window.ClientBounds.Height, SceneManager.GraphicsDevice.Viewport.AspectRatio, Hero.getPOSITION());
            //load model
            Hero.LoadModel(content, SceneManager.GraphicsDevice.Viewport.AspectRatio);
            
            table = new ArenaTable(getStartTile(), content);
            skybox = new ArenaSkybox(getStartTile(), content);

            potionsUsed = 0;

            //load CombatInfo at top left
            Vector2 start = new Vector2(10, 45);
            foreach (PlayerSprite ps in PartyUtils.getParty())
            {
                ps.getCombatInfo().init(content, start);
                start.Y += 60;
            }

            pauseMenu.load(content, SceneManager.GraphicsDevice.Viewport);
            pauseMenu.center();

            // Debug arena
            printDebugArena();
        }

        /**
         * The update function called in each frame.
         */
        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            // Handle pause input
            if (pause.wasPressed(inputState))
                paused = !paused;

            if (!paused)
            {
                //update Character
                Hero.Update(gameTime, inputState);
                camera.UpdateCamera(Hero.getPOSITION());

                // Update the spotlight
                if (flashTimer > 0)
                {
                    flashTimer--;
                }
                else
                {
                    if (!flashOn)
                    {
                        flashOn = true;
                    }
                }

                if (!flashOn)
                {
                    FlashLightAngle += MathUtils.smoothChange(FlashLightAngle, MIN_FLASH, FLASH_RATE);
                }
                else
                {

                    FlashLightAngle += MathUtils.smoothChange(FlashLightAngle, MAX_FLASH, FLASH_RATE);
                }


                // Update the tiles
                for (int i = 0; i < baseArena.GetLength(0); i++)
                {
                    for (int j = 0; j < baseArena.GetLength(1); j++)
                    {
                        baseArena[i, j].Update(gameTime, inputState);
                    }
                }

                // Rotation of camera
                if (rotateUp.wasPressed(inputState))
                {
                    camera.rotateCamera(true);
                }
                else if (rotateDown.wasPressed(inputState))
                {
                    camera.rotateCamera(false);
                }

                //update combatinfo
                foreach (PlayerSprite ps in PartyUtils.getParty())
                {
                    ps.getCombatInfo().updateArena(ps.getStats());
                }

                controller.update();
            }
            else
            {
                if (menu.wasPressed(inputState))
                    SceneManager.changeScene(SceneState.main);
            }
        }

        /**
         * The draw function called at the end of each frame.
         */
        public override void draw(GameTime gameTime)
        {
            SpriteBatch sb = SceneManager.SpriteBatch;
            SceneManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateGray, 1.0f, 0);
            SceneManager.GraphicsDevice.BlendState = BlendState.Opaque;
            SceneManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            BaseGame.instance.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            SceneManager.GraphicsDevice.RasterizerState = rs;
            
            Vector3 LightPos = new Vector3(Hero.getPOSITION().X, Hero.getPOSITION().Y + 400, Hero.getPOSITION().Z + 100);
            Vector3 position = camera.getPosition();
            //Vector3 LAt = camera.getLookAt() - position;
            Vector3 LAt = Hero.getPOSITION() - LightPos;
            HLSLeffect.CurrentTechnique = HLSLeffect.Techniques["ShaderTech"];

            HLSLeffect.Parameters["AmbientColor"].SetValue(ambientColour.ToVector4());
            HLSLeffect.Parameters["AmbientIntensity"].SetValue(ARENA_AMBIENCE);
            HLSLeffect.Parameters["fogColor"].SetValue(Color.Gray.ToVector4());
            HLSLeffect.Parameters["fogFar"].SetValue(1000.0f);
            HLSLeffect.Parameters["fogEnabled"].SetValue(true);
            HLSLeffect.Parameters["FlashlightAngle"].SetValue(MathHelper.ToRadians(FlashLightAngle));

            HLSLeffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(LAt));
            HLSLeffect.Parameters["EyePosition"].SetValue(LightPos);//position

            HLSLeffect.Parameters["View"].SetValue(camera.getView());
            HLSLeffect.Parameters["Projection"].SetValue(camera.getProjection());

            // Draw the skybox
            skybox.Draw(sb, HLSLeffect);

            // Draw the table
            table.Draw(sb, HLSLeffect);

            // Draw the tiles
            for (int i = 0; i < baseArena.GetLength(0); i++)
            {
                for (int j = 0; j < baseArena.GetLength(1); j++)
                {
                    baseArena[i, j].Draw(sb, HLSLeffect);
                }
            }

            //Draw character
            Hero.Draw(sb, HLSLeffect);

            //Draw the combatInfo
            sb.Begin();
            foreach (PlayerSprite ps in PartyUtils.getParty())
            {
                ps.getCombatInfo().drawArena(sb);
            }
            sb.End();

            if(paused)
            {
                sb.Begin();
                pauseMenu.draw(sb, null);
                sb.End();
            }
;
        }

        /**
         * This function is called when a scene is no longer active.
         */
        public override void unloadScene()
        {

        }

        /// <summary>
        /// This function reloads the arena with a new difficulty.
        /// </summary>
        /// <param name="difficulty">The new arena's difficulty</param>
        public void loadNewArena(ArenaDifficulty difficulty)
        {
            collidables = new List<Entity>();

            if (content == null)
                content = new ContentManager(SceneManager.Game.Services, "Content");

            //populate the GameSave object
            GameSave.seed = Environment.TickCount;
            GameSave.level = controller.getLevel();
            GameSave.partyHealth = PartyUtils.getPartyHealth();
            GameSave.score = controller.getScore();
            //SAVE
            SaveUtils.getInstance().saveGame(GameSave);

            controller.setGenerator(new Random(GameSave.seed));

            // Generate the arena
            ArenaBuilder builder = new ArenaBuilder(controller.getLevelSize(controller.getLevel()), controller.getLevelSize(controller.getLevel()),
                content, SceneManager.GraphicsDevice.Viewport.AspectRatio, difficulty);
            baseArena = builder.buildArenaBase();
            StartTile = builder.getStartTile();

            //
            if (effect == null)
                effect = new BasicEffect(SceneManager.Game.GraphicsDevice);//null

            Hero = new Character();
            camera = new Camera(effect, SceneManager.Game.Window.ClientBounds.Width, SceneManager.Game.Window.ClientBounds.Height, SceneManager.GraphicsDevice.Viewport.AspectRatio, Hero.getPOSITION());
            //load model
            Hero.LoadModel(content, SceneManager.GraphicsDevice.Viewport.AspectRatio);
            potionsUsed = 0;

            // Debug arena
            printDebugArena();
        }

        public void startBattle(PartyUtils.Enemy enemy)
        {
            BattleBuilder battleBuilder = new BattleBuilder(enemy);

            // Create and switch to the battle
            BattleScene battle = new BattleScene(battleBuilder.getFront(), battleBuilder.getBack());

            SceneManager.setScene(SceneState.battle, battle, true);
        }


        /// <summary>
        /// method for retrieving the Start Tile;
        /// the tile on which the character initially starts on 
        /// when the arena is first loaded.
        /// </summary>
        /// <returns>The Starting Tile.</returns>
        public Tile getStartTile()
        {
            return StartTile;
        }

        private void printDebugArena()
        {
            for (int i = 0; i < baseArena.GetLength(0); i++)
            {
                for (int j = 0; j < baseArena.GetLength(1); j++)
                {
                    Console.Write(baseArena[i, j].getNumConnections() + " ");
                }

                Console.WriteLine();
            }
        }
    }
}
