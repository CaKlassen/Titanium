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
using Titanium.Scenes.Panels;

namespace Titanium.Scenes
{
    /// <summary>
    /// This class represents a scene in game. Each scene represents a distinct part of the game.
    /// </summary>
    abstract class Scene
    {
        /// <summary>
        /// Gets or sets this scene's SceneManager.
        /// </summary>
        public SceneManager SceneManager
        {
            get { return sceneManager; }
            set { sceneManager = value; }
        }
        SceneManager sceneManager;

        /// <summary>
        /// This function is called when a scene is made active.
        /// </summary>
        /// <param name="content">The content manager</param>
        public virtual void loadScene(ContentManager content) { }

        /// <summary>
        /// This function updates the scene each frame.
        /// </summary>
        /// <param name="gameTime">The game time for timing</param>
        /// <param name="inputState">The input state for input</param>
        public virtual void update(GameTime gameTime, InputState inputState) {  }

        /// <summary>
        /// This function renders the scene.
        /// </summary>
        /// <param name="gameTime">The game time for timing</param>
        public virtual void draw(GameTime gameTime){}

        /// <summary>
        /// This function unloads the scene.
        /// </summary>
        public abstract void unloadScene();

    }
}
