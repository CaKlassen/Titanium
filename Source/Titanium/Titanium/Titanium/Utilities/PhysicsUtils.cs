using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Entities.Traps;
using Titanium.Scenes;

namespace Titanium.Utilities
{
    public static class PhysicsUtils
    {
        /// <summary>
        /// Checks for a collision between the main character and the arena enemy.
        /// </summary>
        /// <param name="Hero">the main character.</param>
        /// <param name="enemy">the enemy in question.</param>
        public static bool CheckCollision(Character Hero, ArenaEnemy enemy)
        {

            for (int i = 0; i < Hero.myModel.Meshes.Count; i++)
            {
                BoundingSphere HeroSphere = Hero.myModel.Meshes[i].BoundingSphere;
                HeroSphere.Center += Hero.getPosition();

                for (int j = 0; j < enemy.myModel.Meshes.Count; j++)
                {
                    BoundingSphere EnemySphere = enemy.myModel.Meshes[j].BoundingSphere;
                    EnemySphere.Center += enemy.getPosition();

                    if (HeroSphere.Intersects(EnemySphere))
                    {
                        //collision!
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks for a collision between the main character and the arena door.
        /// </summary>
        /// <param name="Hero">the main character.</param>
        /// <param name="exit">the arena door.</param>
        public static bool CheckCollision(Character Hero, ArenaExit exit)
        {
            for (int i = 0; i < Hero.myModel.Meshes.Count; i++)
            {
                BoundingSphere HeroSphere = Hero.myModel.Meshes[i].BoundingSphere;
                HeroSphere.Center += Hero.getPosition();

                for (int j = 0; j < exit.myModel.Meshes.Count; j++)
                {
                    BoundingSphere EnemySphere = exit.myModel.Meshes[j].BoundingSphere;
                    EnemySphere.Center += exit.getPosition();

                    if (HeroSphere.Intersects(EnemySphere))
                    {
                        //collision!
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks for a collision between the pojectile and the main character.
        /// </summary>
        /// <param name="Hero">the main character.</param>
        /// <param name="p">the projectile.</param>
        /// <returns></returns>
        public static bool CheckCollision(Character Hero, Projectile p)
        {
            for (int i = 0; i < p.myModel.Meshes.Count; i++)
            {
                BoundingSphere ProjSphere = p.myModel.Meshes[i].BoundingSphere;
                ProjSphere.Center += p.getPosition();

                for (int j = 0; j < Hero.myModel.Meshes.Count; j++)
                {
                    BoundingSphere HeroSphere = Hero.myModel.Meshes[j].BoundingSphere;
                    HeroSphere.Center += Hero.getPosition();

                    if (ProjSphere.Intersects(HeroSphere))
                    {
                        //collision!
                        return true;
                    }
                }
            }
            //no collision
            return false;
        }
    }
}
