using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Scenes;
using Titanium.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Entities.Items
{
    public class Potion : Entity
    {
        public Model myModel;
        private Vector3 Position;
        private float scale = 1f;
        private float modelOrientation = 0f;
        private float HealPercent;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="position">position of the items model</param>
        /// <param name="healPrecentage">precentage of health the potion will heal (between 1 and 100)</param>
        public Potion(Vector3 position, float healPrecentage)
        {
            HealPercent = healPrecentage;
            Position = position;
        }


        public void LoadModel(ContentManager cm)
        {
            myModel = cm.Load<Model>("Models/hero");
        }

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
                        //effect.EnableDefaultLighting();//lighting
                        ArenaScene.instance.camera.SetLighting(effect);
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(MathHelper.ToRadians(modelOrientation))
                            * Matrix.CreateTranslation(Position);
                        effect.View = ArenaScene.instance.camera.getView();
                        effect.Projection = ArenaScene.instance.camera.getProjection();
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {

        }

        public Vector3 getPosition()
        {
            return Position;
        }

        public float getHealPercent()
        {
            return HealPercent;
        }
    }
}
