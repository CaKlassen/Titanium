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

        PartyPanel party;
        EnemyPanel enemies;

        /// <summary>
        /// Initializes a blank Encounter for debug / bug fixing purposes
        /// </summary>
        public Encounter(List<PartyUtils.Enemy> enemyList)
        {
            enemies = new EnemyPanel(PartyUtils.makeEnemies(enemyList));
            party = new PartyPanel();
        }

        /// <summary>
        /// Load the two sprite panels and save a reference to the content manager
        /// </summary>
        /// <param name="content"></param>
        public void load(ContentManager content, Viewport v)
        {
            this.content = content;
            enemies.load(content, v);
            party.load(content, v);
            resolved = true;
        }

        /// <summary>
        /// Draw this encounter
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used</param>
        public void draw(SpriteBatch sb)
        {
            enemies.draw(sb);
            party.draw(sb);
        }

        /// <summary>
        /// This function will detect the current state of the battle and update the necessary components only
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs to be used for input handling</param>
        public void update(GameTime gameTime, InputState inputState)
        {
            enemies.update(gameTime, inputState);
            party.update(gameTime, inputState);
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
