using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Entities.Items;
using Titanium.Entities.Traps;
using Titanium.Scenes;

namespace Titanium.Utilities
{
    public static class PhysicsUtils
    {
        /// <summary>
        /// Generic collision detection function.
        /// Checks collisions between two Entities.
        /// </summary>
        /// <param name="a">the first Entity.</param>
        /// <param name="b">the second Entity.</param>
        /// <returns>True if the two Entities are colliding; false otherwise.</returns>
        public static bool CheckCollision(Entity a, Entity b)
        {
            if (b.GetType() == typeof(MysteryBox))
            {
                return CheckCollision(a, (MysteryBox)b);
            }
            else
            {
                if (a.myModel != null && b.myModel != null)
                {
                    for (int i = 0; i < a.myModel.Meshes.Count; i++)
                    {
                        BoundingSphere HeroSphere = a.myModel.Meshes[i].BoundingSphere;
                        HeroSphere.Center += a.getPOSITION();

                        for (int j = 0; j < b.myModel.Meshes.Count; j++)
                        {
                            BoundingSphere EnemySphere = b.myModel.Meshes[j].BoundingSphere;
                            EnemySphere.Center += b.getPOSITION();

                            if (HeroSphere.Intersects(EnemySphere))
                            {
                                //collision!
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
        }

        public static bool CheckCollision(Entity a, MysteryBox b)
        {

            if (a.myModel != null && b.myModel != null)
            {
                for (int i = 0; i < a.myModel.Meshes.Count; i++)
                {
                    BoundingSphere HeroSphere = a.myModel.Meshes[i].BoundingSphere;
                    HeroSphere.Center += a.getPOSITION();

                    for (int j = 0; j < b.myModel.Meshes.Count; j++)
                    {
                        BoundingSphere EnemySphere = b.myModel.Meshes[j].BoundingSphere.Transform(Matrix.CreateScale(0.2f));
                        EnemySphere.Center += b.getPOSITION();

                        if (HeroSphere.Intersects(EnemySphere))
                        {
                            //collision!
                            return true;
                        }
                    }
                }
            }
            return false;
            
        }
    }
}
