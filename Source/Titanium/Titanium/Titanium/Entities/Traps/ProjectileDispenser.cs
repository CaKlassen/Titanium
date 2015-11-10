using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Scenes;
using Titanium.Arena;

namespace Titanium.Entities.Traps
{
    class ProjectileDispenser : Entity
    {
        //attributes
        public Model ProjModel;
        private Texture2D myTexture;
        //public Model myModel;
        private float scale = 0.5f;
        private float modelOrientation = 0.0f;
        private Vector3 Orientation;
        private Vector3 position;
        private int projdmg;

        private static int FIRE_HEIGHT = 5;
        const float TIMER = 1.5f;
        float timer = TIMER;

        public List<Projectile> Projectiles;

        //attributes
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public int ProjectileDamage
        {
            get { return projdmg; }
            set { projdmg = value; }
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
            Projectiles = new List<Projectile>();
            setModelOrientation();
        }



        public void LoadModel(ContentManager cm)
        {
            myModel = cm.Load<Model>("Models/Dispenser");//change to actual dispenser model
            myTexture = cm.Load<Texture2D>("Models/skyboxBG");//change to actual dispenser model
            ProjModel = cm.Load<Model>("Models/Projectile");//change to actual projectile model
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

            if (Projectiles != null && Projectiles.Count > 0)
            {
                CleanProjectiles();
                foreach(Projectile p in Projectiles)
                {
                    p.Update(gameTime, inputState);
                }
            }

        }
    
        /// <summary>
        /// clean up the list of projectiles; remove dead projectiles.
        /// </summary>
        private void CleanProjectiles()
        {          
            foreach(Projectile p in Projectiles.ToList())
            {
                if(p.Dead)
                {
                    Projectiles.Remove(p);
                }
            }
        }

        /// <summary>
        /// create a new projectile and add it to the list of projectiles.
        /// </summary>
        private void FireProjectile()
        {
            Projectile p = new Projectile(new Vector3(position.X, position.Y + FIRE_HEIGHT, position.Z), Orientation, ProjectileDamage, ProjModel);
            Projectiles.Add(p);
        }


        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(MathHelper.ToRadians(modelOrientation))
                            * Matrix.CreateTranslation(Position);

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

            //draw projectiles fired as well here
            if (Projectiles != null && Projectiles.Count > 0)
            {
                foreach (Projectile p in Projectiles)
                {
                    p.Draw(sb, effect);
                }
            }
        }

        /// <summary>
        /// This function returns the direction of the projectile to be fired based on the tile provided.
        /// </summary>
        /// <param name="tile">The tile type</param>
        /// <returns>The fire vector</returns>
        public static Vector3 getFireDirection(ArenaTiles tile)
        {
            Vector3 dir = new Vector3(0, 0, 0);

            switch(tile)
            {
                case ArenaTiles.CORNER_BL:
                {
                    dir.X = -1;
                    dir.Z = 1;                    
                    break;
                }

                case ArenaTiles.CORNER_BR:
                {
                    dir.X = 1;
                    dir.Z = 1;
                    break;
                }

                case ArenaTiles.CORNER_TL:
                {
                    dir.X = -1;
                    dir.Z = -1;
                    break;
                }

                case ArenaTiles.CORNER_TR:
                {
                    dir.X = 1;
                    dir.Z = -1;
                    break;
                }

                case ArenaTiles.DE_BOTTOM:
                case ArenaTiles.DE_TOP:
                case ArenaTiles.STR_VERT:
                {
                    dir.X = -1;
                    break;
                }

                case ArenaTiles.DE_LEFT:
                case ArenaTiles.DE_RIGHT:
                case ArenaTiles.STR_HOR:
                {
                    dir.Z = -1;
                    break;
                }

                default:
                {
                    dir.Y = 1;
                    break;
                }
            }

            return dir;
        }

        /// <summary>
        /// sets the models orientation
        /// </summary>
        private void setModelOrientation()
        {
            switch((int)Orientation.X)
            {
                case 1:
                {
                    if(Orientation.Z == 1)
                    {
                        modelOrientation = -135f;
                    }
                    
                    if(Orientation.Z == -1)
                    {
                        modelOrientation = -45f;
                    }

                    if(Orientation.Z == 0)
                    {
                        modelOrientation = -90f;
                    }
                    break;
                }
                case -1:
                {
                    if (Orientation.Z == 1)
                    {
                        modelOrientation = 135f;
                    }

                    if (Orientation.Z == -1)
                    {
                        modelOrientation = 45f;
                    }

                    if (Orientation.Z == 0)
                    {
                        modelOrientation = 90f;
                    }
                    break;
                }
                case 0:
                {
                    if (Orientation.Z == 1)
                    {
                        modelOrientation = 180f;
                    }

                    if (Orientation.Z == -1)
                    {
                        modelOrientation = 0f;
                    }
                    break;
                }

            }
        }

        public override Vector3 getPOSITION()
        {
            return position;
        }
    }
}
