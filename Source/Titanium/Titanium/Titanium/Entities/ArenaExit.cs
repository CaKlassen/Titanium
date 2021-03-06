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
using Titanium.Entities;
using Titanium.Arena;
using Titanium.Scenes;

namespace Titanium.Entities
{
    /// <summary>
    /// This class represents the arena exit.
    /// </summary>
    public class ArenaExit : Entity
    {
        private Tile _currentTile;//the current tile we are standing on
        private Vector3 _Position;

        private Texture2D myTexture;
        private Texture2D myTextureOpen;
        //MovableModel
        private float modelRotation;
        //public Model myModel;
        private float scale;

        /// <summary>
        /// This is the default constructor for the arena exit.
        /// </summary>
        /// <param name="createTile">The tile to start on</param>
        /// <param name="Content">The content manager for loading</param>
        public ArenaExit(Tile createTile, ContentManager Content)
        {
            // Add this to the collidables list
            ArenaScene.instance.collidables.Add(this);

            _currentTile = createTile;
            _Position = new Vector3(_currentTile.getModelPos().X, -2, _currentTile.getModelPos().Z); //should start in the middle of the start tile (X, Y, Z);
            

            if (_currentTile.getType() == ArenaTiles.DE_BOTTOM || _currentTile.getType() == ArenaTiles.DE_TOP)
            {
                modelRotation = 1.5708f;
            }
            else
            {
                modelRotation = 0;
            }

            scale = 0.3f;
            
            // Load the model
            myModel = Content.Load<Model>("Models/Exit");
            myTexture = Content.Load<Texture2D>("Models/Exit-UVMap");
            myTextureOpen = Content.Load<Texture2D>("Models/Exit-UVMap-Open");
        }

        /// <summary>
        /// This function updates the arena exit.
        /// </summary>
        /// <param name="gameTime">The game time object for timing</param>
        /// <param name="inputState">The input state object for input</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {

        }

        /// <summary>
        /// This function renders the arena exit to the screen.
        /// </summary>
        /// <param name="sb">The spritebatch object for rendering</param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {

            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(_Position);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            part.Effect = effect;

                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * worldMatrix);
                            effect.Parameters["AmbientIntensity"].SetValue(_currentTile.TileAmbience);

                            if (ArenaController.instance.getNumEnemies() == 0)
                            {
                                effect.Parameters["ModelTexture"].SetValue(myTextureOpen);
                            }
                            else
                            {
                                effect.Parameters["ModelTexture"].SetValue(myTexture);
                            }


                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            effect.Parameters["AmbientIntensity"].SetValue(ArenaScene.ARENA_AMBIENCE);
        }

        /// <summary>
        /// Method to get the characters current position.
        /// </summary>
        /// <returns>The position of the player character as a Vector3.</returns>
        public override Vector3 getPOSITION()
        {
            return _Position;
        }
    }
}
