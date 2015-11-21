using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Titanium.Arena;
using Titanium.Scenes;
using Titanium.Utilities;

namespace Titanium.Entities
{
    /// <summary>
    /// This class represents an enemy in the arena.
    /// </summary>
    public class ArenaEnemy : Entity
    {
        /// <summary>
        /// The rate at which the enemy moves between tiles.
        /// </summary>
        public static int MOVE_RATE = 10;

        /// <summary>
        /// The maximum number of times to try to move.
        /// </summary>
        public static int MAX_TRIES = 20;

        /// <summary>
        /// The number of turns to wait between moves.
        /// </summary>
        public static int WAIT_TURNS = 3;

        private Tile _currentTile;//the current tile we are standing on
        private Vector3 _Position;

        private PartyUtils.Enemy type;

        //MovableModel
        //public Model myModel;
        private float modelRotation = MathHelper.ToRadians(180);

        private Texture2D texture;

        private float scale;

        private int waitTurns = WAIT_TURNS;
        
        /// <summary>
        /// This is the default constructor for the arena enemy.
        /// </summary>
        /// <param name="createTile">The tile to start on</param>
        /// <param name="Content">The content manager for loading</param>
        public ArenaEnemy(Tile createTile, ContentManager Content, PartyUtils.Enemy type)
        {
            // Add this to the collidables list
            ArenaScene.instance.collidables.Add(this);

            _currentTile = createTile;
            _Position = new Vector3(_currentTile.getModelPos().X, 0, _currentTile.getModelPos().Z); //should start in the middle of the start tile (X, Y, Z);
            
            
            scale = 0.5f;

            // Set the wait turns randomly
            waitTurns = ArenaController.instance.getGenerator().Next(1, WAIT_TURNS + 1);

            this.type = type;

            myModel = Content.Load<Model>("Models/hero");

            setTexture(Content);
        }
        
        /// <summary>
        /// This function updates the arena enemy.
        /// </summary>
        /// <param name="gameTime">The game time object for timing</param>
        /// <param name="inputState">The input state object for input</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            // If the player moved this frame
            if (ArenaController.instance.getPlayerMoved())
            {
                waitTurns--;

                // If it's our turn to move
                if (waitTurns == 0)
                {
                    waitTurns = WAIT_TURNS;

                    TileConnections dir = TileConnections.NONE;
                    Random r = ArenaController.instance.getGenerator();

                    // Search for an adjacent tile
                    for (int i = 0; i < MAX_TRIES; i++)
                    {
                        dir = (TileConnections)r.Next(0, 4);

                        if (_currentTile.getConnection(dir) != null &&
                            !_currentTile.getConnection(dir).hasEnemy() &&
                            !_currentTile.getConnection(dir).hasExit())
                        {
                            // We found a good tile
                            break;
                        }
                        else
                        {
                            dir = TileConnections.NONE;
                        }
                    }

                    if (dir != TileConnections.NONE)
                    {
                        // Swap the tile
                        _currentTile.deleteEntity(this);
                        _currentTile = _currentTile.getConnection(dir);
                        _currentTile.addEntity(this);
                    }
                }
            }

            _Position.X += MathUtils.smoothChange(_Position.X, _currentTile.getDrawPos().X, MOVE_RATE);
            _Position.Z += MathUtils.smoothChange(_Position.Z, _currentTile.getDrawPos().Y, MOVE_RATE);
        }

        /// <summary>
        /// Method to get the characters current position.
        /// </summary>
        /// <returns>The position of the player character as a Vector3.</returns>
        public override Vector3 getPOSITION()
        {
            return _Position;
        }


        /// <summary>
        /// This function kills the enemy.
        /// </summary>
        public void die()
        {
            _currentTile.deleteEntity(this);
            ArenaScene.instance.collidables.Remove(this);
        }


        /// <summary>
        /// This function renders the arena enemy to the screen.
        /// </summary>
        /// <param name="sb">The spritebatch object for rendering</param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelRotation)
                            * Matrix.CreateTranslation(_Position);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            part.Effect = effect;

                            effect.Parameters["AmbientIntensity"].SetValue(0.3f);
                            effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * worldMatrix);
                            effect.Parameters["ModelTexture"].SetValue(texture);

                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
                effect.Parameters["AmbientIntensity"].SetValue(ArenaScene.ARENA_AMBIENCE);
            }
        }

        public PartyUtils.Enemy getEnemyType()
        {
            return type;
        }

        /// <summary>
        /// This function sets the texture for the enemy.
        /// </summary>
        /// <param name="Content"></param>
        private void setTexture(ContentManager Content)
        {
            switch(type)
            {
                case PartyUtils.Enemy.Bat:
                {
                    texture = Content.Load<Texture2D>("Models/BatMap");
                    break;
                }

                case PartyUtils.Enemy.Redbat:
                {
                    texture = Content.Load<Texture2D>("Models/RedbatMap");
                    break;
                }

                case PartyUtils.Enemy.Slime:
                {
                    texture = Content.Load<Texture2D>("Models/SlimeMap");
                    break;
                }

                case PartyUtils.Enemy.PoisonSlime:
                {
                    texture = Content.Load<Texture2D>("Models/PoisonSlimeMap");
                    break;
                }

                case PartyUtils.Enemy.Spider:
                {
                    texture = Content.Load<Texture2D>("Models/SpiderMap");
                    break;
                }

                case PartyUtils.Enemy.CinderSpider:
                {
                    texture = Content.Load<Texture2D>("Models/CinderSpiderMap");
                    break;
                }

                default:
                {
                    texture = Content.Load<Texture2D>("Models/BatMap");
                    break;
                }
            }
        }
    }
}
