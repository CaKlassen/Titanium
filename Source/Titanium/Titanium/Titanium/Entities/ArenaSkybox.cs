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
using Titanium.Arena;
using Titanium.Scenes;

namespace Titanium.Entities
{
    /// <summary>
    /// This class represents an arena skybox.
    /// </summary>
    public class ArenaSkybox : Entity
    {
        private Tile _currentTile;//the current tile we are standing on
        private Vector3 _Position;
        private ForwardDir _forward;//1 = up; 2 = right; 3 = down; 4 = left 

        //MovableModel
        private float modelRotation;
        public Model myModel;
        public Texture2D texture;
        private Vector3 modelPosition;
        private float scale;

        /// <summary>
        /// This is the default constructor for the arena skybox.
        /// </summary>
        /// <param name="createTile">The tile to centre on</param>
        /// <param name="Content">The content manager for loading</param>
        public ArenaSkybox(Tile createTile, ContentManager Content)
        {
            _currentTile = createTile;
            _Position = new Vector3(_currentTile.getModelPos().X, 0, _currentTile.getModelPos().Z); //should start in the middle of the start tile (X, Y, Z);

            //_Position = Vector3.Zero;
            _forward = ForwardDir.UP;
            scale = 80f;

            modelRotation = 0.0f;
            modelPosition = new Vector3(_currentTile.getModelPos().X, 0, _currentTile.getModelPos().Z);//models position appears on the start tile.
            
            myModel = Content.Load<Model>("Models/skybox");
            texture = Content.Load<Texture2D>("Models/skyboxBG");
        }

        /// <summary>
        /// This function updates the arena skybox.
        /// </summary>
        /// <param name="gameTime">The game time object for timing</param>
        /// <param name="inputState">The input state object for input</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {

        }

        /// <summary>
        /// This function renders the arena skybox to the screen.
        /// </summary>
        /// <param name="sb">The spritebatch object for rendering</param>
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
                        effect.EnableDefaultLighting();

                        // Set the tile's UV Map
                        effect.TextureEnabled = true;
                        effect.Texture = texture;

                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelRotation)
                            * Matrix.CreateTranslation(modelPosition);
                        effect.View = ArenaScene.instance.camera.getView();//Matrix.CreateLookAt(cameraPosition, Target, Vector3.Up);//Vector3.Zero
                        effect.Projection = ArenaScene.instance.camera.getProjection();//Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                                                                                       //aspectRatio, 1.0f, 10000.0f);//1366/768
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }
    }
}
