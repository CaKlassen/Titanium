using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Utilities;

namespace Titanium.Scenes
{
    public enum SceneState
    {
        main,
        arena,
        battle,
        highScores
    }

    /// <summary>
    /// Class that manages the scenes registered to each game state and 
    /// renders them accordingly. Scenes registered 
    /// </summary>
    class SceneManager : DrawableGameComponent
    {
        public static int NUM_SCENESTATES = 4;
        public static int WAIT_TIME = 15;

        Scene[] scenes = new Scene[NUM_SCENESTATES];

        SceneState currentScene;
        SceneState nextScene;
        Texture2D curtainsLeft;
        Texture2D curtainsRight;
        static int transitionInSpeed = 9;
        static int transitionOutSpeed = 12;
        float transitionPosition;
        int midPoint;
        InputState input = new InputState();
        private int waitTime = WAIT_TIME;

        SpriteBatch spriteBatch;
        SpriteFont font;
        ContentManager content;

        bool isInitialized;

        enum State
        {
            transitionOff,
            transitionOn,
            wait,
            active
        }
        State state;

        /// <summary>
        /// A default SpriteBatch shared by all the scenes. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        
        /// <summary>
        /// A default font shared by all the scenes. This saves
        /// each screen having to bother loading their own local copy.
        /// </summary>
        public SpriteFont Font
        {
            get { return font; }
        }

        /// <summary>
        /// Constructs a new SceneManager.
        /// </summary>
        public SceneManager(Game game): base(game)
        {
            registerScene(new MainMenuScene(), SceneState.main);

        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            content = Game.Content;

            InputAction.Load(content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("TestFont");
            curtainsLeft = content.Load<Texture2D>("Sprites/Curtain-Left");
            curtainsRight = content.Load<Texture2D>("Sprites/Curtain-Right");

            midPoint = curtainsLeft.Width;//GraphicsDevice.Viewport.Width / 2;

            // Tell each of the screens to load their content.
            foreach (Scene scene in scenes)
            {
                if( scene != null )
                    scene.loadScene(content);
            }

            currentScene = SceneState.main;
            nextScene = SceneState.main;
            state = State.wait;
            transitionPosition = midPoint;
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (Scene scene in scenes)
            {
                if(scene != null)
                    scene.unloadScene();
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void registerScene(Scene scene, SceneState state)
        {
            scene.SceneManager = this;

            // If we have a graphics device, tell the scene to load content.
            if (isInitialized)
            {
                scene.loadScene(content);
            }

            scenes[(int)state] = scene;
        }

        /// <summary>
        /// Allows the current scene to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.update();

            
            

            switch (state)
            {
                case State.transitionOff:
                    transitionPosition += MathUtils.smoothChange(transitionPosition, midPoint+1, transitionInSpeed);
                    if (transitionPosition >= midPoint - 1f)
                    {
                        transitionPosition = midPoint;
                        state = State.wait;
                    }
                    break;
                case State.wait:
                    if (waitTime > 0)
                    {
                        waitTime--;
                    }
                    else
                    {
                        waitTime = WAIT_TIME;
                        state = State.transitionOn;
                        currentScene = nextScene;
                        if (scenes[(int)currentScene] != null)
                            scenes[(int)currentScene].update(gameTime, input);
                    }
                    break;
                case State.transitionOn:
                    transitionPosition += MathUtils.smoothChange(transitionPosition, -1, transitionOutSpeed);
                    if (transitionPosition <= 1f)
                    {
                        state = State.active;
                        transitionPosition = 0;
                    }
                    if (scenes[(int)currentScene] != null)
                        scenes[(int)currentScene].update(gameTime, input);
                    break;
                default:
                    if (scenes[(int)currentScene] != null)
                        scenes[(int)currentScene].update(gameTime, input);
                    break;
            }

        }

        /// <summary>
        /// Tells the current scene to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            
            if (scenes[(int)currentScene] != null)
                scenes[(int)currentScene].draw(gameTime);

            switch (state)
            {
                case State.transitionOff:
                case State.wait:
                case State.transitionOn:
                    SpriteBatch.Begin();
                    SpriteBatch.Draw(curtainsRight, new Vector2(GraphicsDevice.Viewport.Width - transitionPosition, 0), Color.White);
                    SpriteBatch.Draw(curtainsLeft, new Vector2(transitionPosition - midPoint, 0), Color.White);
                    SpriteBatch.End();
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Associates a SceneState with a Scene and optionally switches to that scene.
        /// </summary>
        /// <param name="state">The desired ScreenState to associate with the Scene</param>
        /// <param name="scene">The desired Scene</param>
        /// <param name="change">true if the game should switch to that scene</param>
        public void setScene(SceneState state, Scene scene, bool change)
        {
            registerScene(scene, state);

            if(change)
                changeScene(state);
        }

        /// <summary>
        /// Change the scene.
        /// </summary>
        /// <param name="scene">The scene state to transition to</param>
        //TODO: animate the scene transitions
        public void changeScene(SceneState scene)
        {
            this.state = State.transitionOff;
            nextScene = scene;
        }
    }
}