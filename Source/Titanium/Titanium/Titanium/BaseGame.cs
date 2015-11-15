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
using Titanium.Scenes;
using Titanium.Entities;
using Titanium.Utilities;
using Microsoft.Xna.Framework.Storage;

namespace Titanium
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BaseGame : Microsoft.Xna.Framework.Game
    {
        public static int SCREEN_WIDTH = 1280;
        public static int SCREEN_HEIGHT = 720;

        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;

        /** An instance to the game object. */
        public static BaseGame instance;

        // Scene manager instance
        SceneManager sceneManager;


        /// <summary>
        /// The base constructor for the game.
        /// </summary>
        public BaseGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            instance = this;

            PartyUtils.loadPartyMembers();
            sceneManager = new SceneManager(this);

            // Set default window properties
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the save utilities
            SaveUtils.getInstance();

            Components.Add(sceneManager);



            base.Initialize();
        }

        protected override void UnloadContent(){}

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        //private StorageDevice Sdevice;
        //private IAsyncResult result;
        //private PlayerIndex playerIndex = PlayerIndex.One;

        //public StorageDevice getStorage()
        //{
        //    if (!Guide.IsVisible)
        //    {
        //        Sdevice = null;//reset device                
        //        result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);//storage device selected
        //        Sdevice = StorageDevice.EndShowSelector(result);//set storage device
        //    }
        //    return Sdevice;
        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }

    }
}
