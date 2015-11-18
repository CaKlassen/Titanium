using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Scenes;
using Titanium.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Entities.Traps
{
    public class Spikes : Entity
    {
        //public Model myModel;
        private Vector3 Position;
        private Texture2D myTexture;
        private float scale = 0.1f;
        private float modelOrientation = 0f;
        private bool collisions;

        const float TIMER = 1.5f;
        float timer = TIMER;
        private static float SPIKES_LOWERED = -30;
        private static float SPIKES_RAISED = -5;
        /// <summary>
        /// contstructor.
        /// </summary>
        /// <param name="position">position of spike trap.</param>
        public Spikes(Vector3 position)
        {
            Position = position;
            Position.Y = SPIKES_LOWERED;//spikes lowered
            collisions = false;
        }

        public void LoadModel(ContentManager cm)
        {
            myModel = cm.Load<Model>("Models/Spikes");
            myTexture = cm.Load<Texture2D>("Models/Spikes-UVMap");
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateTranslation(Position);

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
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float change = 0;
            //spikes up or down based on users movement.
            if (ArenaScene.instance.Hero.getSteps() % 3 == 0)
            {
                //character has taken 3 steps
                change = SPIKES_RAISED;//raise spikes
                collisions = true;//turn on collision detection
            }else
            {
                //character is not on a 3rd step
                collisions = false;//turn off collision detection so no accidental collision are detected.
                change = SPIKES_LOWERED;//lower spikes
                timer = 0.5f;//reset spike damage timer
            }

            Position.Y += MathUtils.smoothChange(Position.Y, change, 5);

            if(collisions)
            {
                //check collision
                if (PhysicsUtils.CheckCollision(ArenaScene.instance.Hero, this))
                {                    
                    timer -= elapsedTime;
                    if (timer <= 0)//standing on the spikes will inflict damage every TIMER amount of seconds
                    {
                        PartyUtils.inflictPartyDamage(15);
                        //Console.Write("spikes! Health -1!\n");
                        timer = TIMER;
                    }
                }     
            }
        }

        /// <summary>
        /// method that returns the models position
        /// </summary>
        /// <returns></returns>
        public override Vector3 getPOSITION()
        {
            return Position;
        }
    }
}
