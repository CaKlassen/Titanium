using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Scenes;
using Titanium.Utilities;
using Microsoft.Xna.Framework.Content;
using Titanium.Arena;

namespace Titanium.Entities.Items
{
    public class Potion : Entity
    {
        //public Model myModel;
        private Vector3 Position;
        private Texture2D myTexture;
        private float scale = 0.5f;
        private float modelOrientation = 0f;
        private float HealPercent;
        private Tile  PotionTile;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="position">position of the items model</param>
        /// <param name="healPrecentage">precentage of health the potion will heal (between 1 and 100)</param>
        /// <param name="potionTile">The tile the potion is on.</param>
        public Potion(Vector3 position, float healPrecentage, Tile potionTile)
        {
            // Add this to the collidables list
            ArenaScene.instance.collidables.Add(this);

            HealPercent = healPrecentage;
            Position = position;
            PotionTile = potionTile;
        }


        public void LoadModel(ContentManager cm)
        {
            myModel = cm.Load<Model>("Models/Potion");
            myTexture = cm.Load<Texture2D>("Models/Potion-UVMap");
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(Position);

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
                            effect.Parameters["ModelTexture"].SetValue(myTexture);

                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
                    }

                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {

        }

        /// <summary>
        /// return the tile the potion is on.
        /// </summary>
        /// <returns>the tile the potion is on.</returns>
        public Tile getTile()
        {
            return PotionTile;
        }

        public override Vector3 getPOSITION()
        {
            return Position;
        }

        public float getHealPercent()
        {
            return HealPercent;
        }

    }
}
