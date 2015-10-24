﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Scenes;

namespace Titanium.Entities.Traps
{
    public class Projectile : Entity
    {
        //attributes
        public Model myModel;
        private float scale = 1f;
        private float modelOrientation = 0.0f;

        Vector3 position;
        static Vector3 VELOCITY = new Vector3(10,10,10);

        private int damage;
        

        /// <summary>
        /// creates new projectile based on starting position and
        /// direction fired in.
        /// The direction is given as either a positive or negative ZUnit, YUnit, or XUnit
        /// that is multiplied with the velocity to produce the projectiles correct tranlation direction.
        /// 
        /// example:
        /// direction = (1,0,0)
        /// VELOCITY = (10,10,10)
        /// direction * VELOCITY = (10,0,0)
        /// the projectile will travel in the positive X direction.
        /// </summary>
        /// <param name="position">Initial spawn position.</param>
        /// <param name="direction">Direction of projectile given by a Vector3.</param>
        /// <param name="damage">amount of damage this projectile inflicts.</param>
        public Projectile(Vector3 position, Vector3 direction, int damage)
        {
            this.position = position;
            VELOCITY *= direction;
            this.damage = damage; 
        }

        public void LoadModel(ContentManager cm, float aspectRatio)
        {
            myModel = cm.Load<Model>("Models/hero");//change to actual model
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
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelOrientation)
                            * Matrix.CreateTranslation(position);
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
            position += VELOCITY;
        }

    }
}