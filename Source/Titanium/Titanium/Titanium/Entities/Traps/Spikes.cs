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
        public Model myModel;
        private Vector3 Position;
        private float scale = 1f;
        private float modelOrientation = 0f;
        private bool collisions;

        const float TIMER = 1.5f;
        float timer = TIMER;
        private static float SPIKES_LOWERED = -10;
        private static float SPIKES_RAISED = 0;
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
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(MathHelper.ToRadians(MathHelper.ToRadians(modelOrientation)))
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
                        PartyUtils.inflictPartyDamage(1);
                        Console.Write("spikes! Health -1!\n");
                        timer = TIMER;
                    }
                }     
            }
        }

        /// <summary>
        /// method that returns the models position
        /// </summary>
        /// <returns></returns>
        public Vector3 getPosition()
        {
            return Position;
        }
    }
}
