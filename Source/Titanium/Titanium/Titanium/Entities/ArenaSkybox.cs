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
        //public Model myModel;
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
            scale = 80f;
            
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
        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(modelPosition);

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
                            effect.Parameters["ModelTexture"].SetValue(texture);

                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        public override Vector3 getPOSITION()
        {
            return _Position;
        }
    }
}
