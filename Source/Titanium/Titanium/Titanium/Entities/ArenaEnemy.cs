﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Titanium.Arena;
using Titanium.Scenes;
using Titanium.Utilities;

namespace Titanium.Entities
{
    /**
     * This class provides a base for all in-game entities that must be updated and rendered to the screen.
     */
    public class ArenaEnemy : Entity
    {
        /// <summary>
        /// The rate at which the enemy moves between tiles.
        /// </summary>
        public static int MOVE_RATE = 10;

        /// <summary>
        /// The number of turns to wait between moves.
        /// </summary>
        public static int WAIT_TURNS = 3;

        private Tile _currentTile;//the current tile we are standing on
        private Vector3 _Position;
        private ForwardDir _forward;//1 = up; 2 = right; 3 = down; 4 = left 

        //MovableModel
        public Model myModel;
        private float modelRotation = 0;
        
        private float scale;

        private int waitTurns = WAIT_TURNS;
        private bool dead = false;
        
        /**
         * The default entity constructor.
         */
        public ArenaEnemy(Tile createTile, ContentManager Content)
        {
            // Add this to the collidables list
            ArenaScene.instance.collidables.Add(this);

            _currentTile = createTile;
            _Position = new Vector3(_currentTile.getModelPos().X, 0, _currentTile.getModelPos().Z); //should start in the middle of the start tile (X, Y, Z);

            //_Position = Vector3.Zero;
            _forward = ForwardDir.UP;
            
            scale = 0.5f;

            // Set the wait turns randomly
            waitTurns = ArenaController.instance.getGenerator().Next(1, WAIT_TURNS + 1);

            myModel = myModel = Content.Load<Model>("Models/enemy");
        }
        
        /**
         * The update function called in each frame.
         */
        public override void Update(GameTime gameTime, InputState inputState)
        {
            // If the player moved this frame
            if (ArenaController.instance.getPlayerMoved())
            {
                waitTurns--;

                // If it's our turn to move
                if (waitTurns == 0)
                {
                    waitTurns = WAIT_TURNS;

                    TileConnections dir;
                    Random r = ArenaController.instance.getGenerator();

                    // Search for an adjacent tile
                    do
                    {
                        dir = (TileConnections)r.Next(0, 4);
                    } while (_currentTile.getConnection(dir) == null);

                    // Swap the tile
                    _currentTile.deleteEntity(this);
                    _currentTile = _currentTile.getConnection(dir);
                    _currentTile.addEntity(this);
                }
            }

            _Position.X += MathUtils.smoothChange(_Position.X, _currentTile.getDrawPos().X, MOVE_RATE);
            _Position.Z += MathUtils.smoothChange(_Position.Z, _currentTile.getDrawPos().Y, MOVE_RATE);
        }

        /// <summary>
        /// Method to get the characters current position.
        /// </summary>
        /// <returns>The position of the player character as a Vector3.</returns>
        public Vector3 getPosition()
        {
            return _Position;
        }


        /// <summary>
        /// This function kills the enemy.
        /// </summary>
        public void die()
        {
            dead = true;
            _currentTile.deleteEntity(this);
            ArenaScene.instance.collidables.Remove(this);
        }


        /**
         * The draw function called at the end of each frame.
         */
        public override void Draw(SpriteBatch sb)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[myModel.Bones.Count];
                myModel.CopyAbsoluteBoneTransformsTo(transforms);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {

                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        ArenaScene.instance.camera.SetLighting(effect);
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelRotation)
                            * Matrix.CreateTranslation(_Position);
                        effect.View = ArenaScene.instance.camera.getView();
                        effect.Projection = ArenaScene.instance.camera.getProjection();
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }
    }
}
