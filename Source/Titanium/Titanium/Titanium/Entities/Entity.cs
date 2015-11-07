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

namespace Titanium.Entities
{
    /// <summary>
    /// This class represents an abstract entity in-game.
    /// </summary>
    public abstract class Entity
    {
        public Model myModel;
        /// <summary>
        /// The default entity constructor
        /// </summary>
        public Entity()
        {

        }
        
        /// <summary>
        /// This function updates the entity.
        /// </summary>
        /// <param name="gameTime">The game time for timing</param>
        /// <param name="inputState">The input state for input</param>
        public abstract void Update(GameTime gameTime, InputState inputState);

        /// <summary>
        /// This function renders the entity to the screen.
        /// </summary>
        /// <param name="sb">The spritebatch for rendering</param>
        public abstract void Draw(SpriteBatch sb);

        /// <summary>
        /// This function returns the entities position
        /// </summary>
        /// <returns>position of the Entity</returns>
        public abstract Vector3 getPOSITION();
    }
}
