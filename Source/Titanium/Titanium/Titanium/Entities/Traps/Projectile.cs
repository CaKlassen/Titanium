using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Scenes;
using Titanium.Utilities;

namespace Titanium.Entities.Traps
{
    public class Projectile : Entity
    {
        //attributes
        //public Model myModel;
        private float scale = 0.5f;
        private float modelOrientation = 0.0f;
        private Boolean dead;

        Vector3 position;
        private Texture2D myTexture;

        private static float AbsVel = 5; //absolute value of velocity regardless of direction
        private Vector3 velocity = new Vector3(AbsVel, 0, AbsVel);

        public static int LIFE_SPAN = 110;
        float lifeSpan = LIFE_SPAN;
        


        private int damage;
        
        public Boolean Dead
        {
            get { return dead; }
            set { dead = value; }
        }

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
        public Projectile(Vector3 position, Vector3 direction, int damage, Model m)
        {
            this.position = position;
            velocity *= direction;
            this.damage = damage;
            dead = false;
            myModel = m;
        }

        public void LoadModel(ContentManager cm, float aspectRatio)
        {
            myTexture = cm.Load<Texture2D>("Models/skyboxBG");//change to actual model
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

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
            //float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //lifeSpan -= elapsedTime;
            lifeSpan -= AbsVel;
            position += velocity;

            //if collision true
            if(PhysicsUtils.CheckCollision(ArenaScene.instance.Hero, this))
            {
                //take away life and set death flag
                PartyUtils.inflictPartyDamage(1);
                dead = true;
            }else if (lifeSpan <= 0)
            {
                //fire projectile
                dead = true;
            }
        }

        public override Vector3 getPOSITION()
        {
            return position;
        }

    }
}
