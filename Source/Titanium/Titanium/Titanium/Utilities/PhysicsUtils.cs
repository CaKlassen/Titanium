using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
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
        public static void CheckCollision(Character Hero, ArenaEnemy enemy)
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
                        Console.Write("COLLISION!");
                    }
                }
            }
        }

        /// <summary>
        /// Checks for a collision between the main character and the arena door.
        /// </summary>
        /// <param name="Hero">the main character.</param>
        /// <param name="exit">the arena door.</param>
        public static void CheckCollision(Character Hero, ArenaExit exit)
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
                        Console.Write("EXIT!");
                    }
                }
            }
        }
    }
}
