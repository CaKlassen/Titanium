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

        // Possible user actions
        InputAction menu,
                    battle;

        ContentManager content;
        public Effect HLSLeffect;

        public ArenaController controller;
        public Character Hero;
        public Camera camera;
        private ArenaTable table;
        private ArenaSkybox skybox;
        private BasicEffect effect;
        public int potionsUsed;

        public float FlashLightAngle;
        public static float ARENA_AMBIENCE = 0.080f;

        public List<Entity> collidables;

        /**
         * The default scene constructor.
         */
        public ArenaScene() : base()
        {
            instance = this;

            // Create the arena controller
            controller = new ArenaController();

            // Define the user actions
            menu = InputAction.SELECT;

            battle = InputAction.X;
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

            // Generate the arena
            ArenaBuilder builder = new ArenaBuilder(6, 6, content, SceneManager.GraphicsDevice.Viewport.AspectRatio, ArenaDifficulty.EASY);
            baseArena = builder.buildArenaBase();
            StartTile = builder.getStartTile();

            //
            if (effect == null)
                effect = new BasicEffect(SceneManager.Game.GraphicsDevice);//null

            FlashLightAngle = 10f;
            Hero = new Character();
            camera = new Camera(effect, SceneManager.Game.Window.ClientBounds.Width, SceneManager.Game.Window.ClientBounds.Height, SceneManager.GraphicsDevice.Viewport.AspectRatio, Hero.getPOSITION());
            //load model
            Hero.LoadModel(content, SceneManager.GraphicsDevice.Viewport.AspectRatio);
            
            table = new ArenaTable(getStartTile(), content);
            skybox = new ArenaSkybox(getStartTile(), content);

            potionsUsed = 0;

            
            // Debug arena
            printDebugArena();
        }

        /**
         * The update function called in each frame.
         */
        public override void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            //update Character
            Hero.Update(gameTime, inputState);
            camera.UpdateCamera(Hero.getPOSITION());

            
            // Update the tiles
            for (int i = 0; i < baseArena.GetLength(0); i++)
            {
                for (int j = 0; j < baseArena.GetLength(1); j++)
                {
                    baseArena[i, j].Update(gameTime, inputState);
                }
            }

            if (menu.Evaluate(inputState, null, out player))
            {
                SceneManager.changeScene(SceneState.main);
            }
            else if (battle.Evaluate(inputState, null, out player))
            {
                SceneManager.changeScene(SceneState.battle);
            }

            controller.update();

            // TEMP: New arena
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                ArenaController.instance.moveToNextArena();
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
            
            Vector3 LightPos = new Vector3(Hero.getPOSITION().X, Hero.getPOSITION().Y + 400, Hero.getPOSITION().Z);//test light pos
            Vector3 position = camera.getPosition();
            //Vector3 LAt = camera.getLookAt() - position;
            Vector3 LAt = Hero.getPOSITION() - LightPos;
            HLSLeffect.CurrentTechnique = HLSLeffect.Techniques["ShaderTech"];

            HLSLeffect.Parameters["AmbientColor"].SetValue(Color.Gold.ToVector4());
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

            // Generate the arena
            ArenaBuilder builder = new ArenaBuilder(6, 6, content, SceneManager.GraphicsDevice.Viewport.AspectRatio, difficulty);
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
