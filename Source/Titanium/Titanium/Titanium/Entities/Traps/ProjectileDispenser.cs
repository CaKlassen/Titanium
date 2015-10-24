﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Scenes;

namespace Titanium.Entities.Traps
{
    class ProjectileDispenser : Entity
    {
        //attributes
        public Model myModel;
        private float scale = 1f;
        private float modelOrientation = 0.0f;
        private Vector3 Orientation;

        float timer = 4;
        const float TIMER = 4;

        public List<Projectile> Projectiles;

        public int ProjectileDamage
        {
            get { return ProjectileDamage; }
            set { ProjectileDamage = value; }
        }

        /// <summary>
        /// constructor.
        /// Orientation is give as a Vector3 only to understand the direction
        /// the dispenser is pointing in; this helps set up model orientation as well
        /// as helping the projectiles fire in the correct direction.
        /// 
        /// example:
        /// (0,0,1) would mean the orientation of the dispenser is facing in the positive Z direction,
        /// therefore the projectiles will fire in the positive Z direction.
        /// </summary>
        /// <param name="position">Vector3 position of the dispenser on the board.</param>
        /// <param name="orientation">direction the dispenser is facing.</param>
        /// <param name="projectileDamage">the damage this dispensers projectiles should inflict.</param>
        public ProjectileDispenser(Vector3 position, Vector3 orientation, int projectileDamage)
        {
            Position = position;
            Orientation = orientation;
            ProjectileDamage = projectileDamage;
        }

        //attributes
        public Vector3 Position
        {
            get { return Position; }
            set { Position = value; }
        }

        public void LoadModel(ContentManager cm, float aspectRatio)
        {
            myModel = cm.Load<Model>("Models/hero");//change to actual model
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsedTime;

            if (timer < 0)
            {
                //fire projectile
                FireProjectile();
                timer = TIMER;
            }
        }
    
        private void FireProjectile()
        {
            Projectile p = new Projectile(Position, Orientation, ProjectileDamage);
            Projectiles.Add(p);
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
                            * Matrix.CreateTranslation(Position);
                        effect.View = ArenaScene.instance.camera.getView();
                        effect.Projection = ArenaScene.instance.camera.getProjection();
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }

            //draw projectiles fired as well here
            if (Projectiles != null && Projectiles.Count > 0)
            {
                foreach (Projectile p in Projectiles)
                {
                    p.Draw(sb);
                }
            }
        }
    }
}
