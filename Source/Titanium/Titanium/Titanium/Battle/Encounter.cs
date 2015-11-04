using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Gambits;
using Titanium.Scenes.Panels;
using Titanium.Utilities;

namespace Titanium.Battle
{
    /// <summary>
    /// This object represents a single "battle" between the player and a set of enemies
    /// </summary>
    public class Encounter
    {

        ContentManager content;

        bool resolved;

        /// <summary>
        /// Initializes a blank Encounter for debug / bug fixing purposes
        /// </summary>
        public Encounter(List<PartyUtils.Enemy> enemyList)
        {
        }

        /// <summary>
        /// Load the two sprite panels and save a reference to the content manager
        /// </summary>
        /// <param name="content"></param>
        public void load(ContentManager content)
        {
            this.content = content;
            resolved = true;
        }

        /// <summary>
        /// Draw this encounter
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used</param>
        public void draw(SpriteBatch sb)
        {

        }

        /// <summary>
        /// This function will detect the current state of the battle and update the necessary components only
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs to be used for input handling</param>
        public void update(GameTime gameTime, InputState inputState)
        {

        }



        public bool success()
        {
            if (!resolved)
            {
                resolved = true;
                return true;
            }
            return false;            
        }

        public bool failure()
        {
            if (!resolved)
            {
                resolved = true;
                return true;
            }

            return false;
        }
    }
}
