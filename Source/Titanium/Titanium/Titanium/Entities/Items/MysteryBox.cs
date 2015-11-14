using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Arena;
using Titanium.Utilities;
using Titanium.Scenes;

namespace Titanium.Entities.Items
{
    public enum MysteryOptions
    {
        HEAL,
        DAMAGE,
        NONE
    }

    public class MysteryBox : Entity
    {
        private Texture2D texture;
        private float scale;
        private Vector3 ModelPos;
        private float modelRotation;
        private Tile MBtile;
        private static float BOX_LIGHT_AMBIENCE = 0.5f;
        private static float BOX_DARK_AMBIENCE = 0.085f;
        private float BoxAmbience;
        private bool dark;
        private MysteryOptions[] outcomes;

        private static float HEAL_PROB = .45f;
        private static float DAMAGE_PROB = .45f;
        private static float NONE_PROB = .10f;

        public MysteryBox(Vector3 position, Tile MysteryBoxTile)
        {
            scale = 0.5f;
            ModelPos = position;
            modelRotation = 0f;
            MBtile = MysteryBoxTile;
            BoxAmbience = BOX_DARK_AMBIENCE;
        }

        /// <summary>
        /// load the mysterbox model and texture
        /// </summary>
        /// <param name="cm">the content manager</param>
        public void LoadModel(ContentManager cm)
        {
            myModel = cm.Load<Model>("Models/hero");
            texture = cm.Load<Texture2D>("Models/PlayerMap");
        }

        /// <summary>
        /// Method to render the mystery box
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        /// <param name="effect">Shader Effect</param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(ModelPos);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            part.Effect = effect;
                            effect.Parameters["AmbientIntensity"].SetValue(BoxAmbience);
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * worldMatrix);
                            effect.Parameters["ModelTexture"].SetValue(texture);

                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
                glow();
            }
            effect.Parameters["AmbientIntensity"].SetValue(ArenaScene.ARENA_AMBIENCE);
        }

        /// <summary>
        /// retrieve Model Position on map
        /// </summary>
        /// <returns>the models position</returns>
        public override Vector3 getPOSITION()
        {
            return ModelPos;
        }

        /// <summary>
        /// update method;
        /// not applicable to mystery box
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="inputState"></param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            //NA
        }

        /// <summary>
        /// get the tile the MysteryBox is on
        /// </summary>
        /// <returns>tile the MysteryBox is on</returns>
        public Tile getTile()
        {
            return MBtile;
        }

        /// <summary>
        /// changes the ambience between light and dark
        /// to produce a glowing effect
        /// </summary>
        private void glow()
        {
            if (BoxAmbience >= BOX_LIGHT_AMBIENCE)
                dark = false;
            else if (BoxAmbience <= BOX_DARK_AMBIENCE)
                dark = true;

            if (dark)
            {
                BoxAmbience += MathUtils.smoothChange(BOX_DARK_AMBIENCE, BOX_LIGHT_AMBIENCE, 100);
            }
            else
                BoxAmbience += MathUtils.smoothChange(BOX_LIGHT_AMBIENCE, BOX_DARK_AMBIENCE, 100);
        }

        /// <summary>
        /// sets up the oucomes array
        /// </summary>
        private void setupOutcomes()
        {
            int heals = (int)(100 * HEAL_PROB);
            int damage = (int)(100 * DAMAGE_PROB);
            int none = (int)(100 * NONE_PROB);

            List<MysteryOptions> temp = new List<MysteryOptions>();

            for (int i = 0; 0 < heals; i++)
            {
                temp.Add(MysteryOptions.HEAL);
            }

            for (int i = 0; 0 < damage; i++)
            {
                temp.Add(MysteryOptions.DAMAGE);
            }

            for (int i = 0; 0 < none; i++)
            {
                temp.Add(MysteryOptions.NONE);
            }

            outcomes = temp.ToArray();
            ShuffleArray(outcomes);

        }

        /// <summary>
        /// Fisher-Yates shuffle
        /// </summary>
        /// <param name="array">the array to shuffle</param>
        private void ShuffleArray(MysteryOptions[] array)
        {
            Random rng = new Random();
            int n = array.Length;
            for (int i = 0; i < n; i++)
            {
                int r = i + (int)(rng.NextDouble() * (n - i));
                MysteryOptions t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        /// <summary>
        /// Calculates randomly what the 
        /// outcome is.
        /// </summary>
        public void RevealMystery()
        {
            Random rng = new Random();
            int result = rng.Next(0, 101);

            switch(outcomes[result])
            {
                case MysteryOptions.HEAL:
                    PartyUtils.HealParty(25);
                    break;

                case MysteryOptions.DAMAGE:
                    PartyUtils.inflictPartyDamage(3);
                    break;

                case MysteryOptions.NONE:
                    break;
            }
        }
    }
}
