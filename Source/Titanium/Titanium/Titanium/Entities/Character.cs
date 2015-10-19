﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Titanium.Arena;
using Titanium.Scenes;
using Titanium.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Entities
{
    public enum ForwardDir {UP, DOWN, LEFT, RIGHT};
    public class Character : Entity
    {
        /// <summary>
        /// The rate at which the player moves between tiles.
        /// </summary>
        public static int MOVE_RATE = 10;

        /// <summary>
        /// The minimum distance from the center of the destination target before the player can move again.
        /// </summary>
        public static float MIN_MOVE_DIS = 20;

        private Tile _StartTile;//the inital starting tile
        private Tile _currentTile;//the current tile we are standing on
        private Vector3 _Position;
        private ForwardDir _forward;//1 = up; 2 = right; 3 = down; 4 = left 

        //MovableModel
        private float aspectRatio, modelRotation;
        public Model myModel;
        //public Matrix ModelMatrix;
        //private SpriteBatch spriteBatch;
        //private String modelPath;
        private Vector3 modelPosition;
        
        private float rotAngle;
        private float scale;

        // Possible player actions
        static InputAction up, down, left, right;
        static Character()
        {
            up = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickUp, Buttons.DPadUp },
                new Keys[] { Keys.W, Keys.Up },
                true
                );

            down = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickDown, Buttons.DPadDown },
                new Keys[] { Keys.S, Keys.Down },
                true
                );

            left = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickLeft, Buttons.DPadLeft },
                new Keys[] { Keys.A, Keys.Left },
                true
                );

            right = new InputAction(
                new Buttons[] { Buttons.LeftThumbstickRight, Buttons.DPadRight },
                new Keys[] { Keys.D, Keys.Right },
                true
                );
        }

        /// <summary>
        /// default constructor.
        /// </summary>
        public Character()
        {
            _StartTile = ArenaScene.instance.getStartTile();
            _currentTile = _StartTile;
            _Position = new Vector3(_StartTile.getModelPos().X, 0, _StartTile.getModelPos().Z); //should start in the middle of the start tile (X, Y, Z);
            
            _forward = ForwardDir.UP;

            rotAngle = 0;
            scale = 0.5f;
            
            modelRotation = 0.0f;

            myModel = null;


        }

        public void LoadModel(ContentManager cm, float aspectRatio)
        {
            myModel = cm.Load<Model>("Models/hero");
            this.aspectRatio = aspectRatio;
        }

        /// <summary>
        /// inherited draw method from Entity class.
        /// </summary>
        /// <param name="sb"></param>
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
                        effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateScale(scale, scale, scale)* Matrix.CreateRotationY(modelRotation)
                            * Matrix.CreateTranslation(_Position);
                        effect.View = ArenaScene.instance.camera.getView();
                        effect.Projection = ArenaScene.instance.camera.getProjection();
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }
            }
        }

        /// <summary>
        /// inherited update method from Entity class.
        /// Move character here.
        /// </summary>
        /// <param name="gamepadState"></param>
        /// <param name="keyboardState"></param>
        /// <param name="mouseState"></param>
        public override void Update(GameTime gameTime, InputState inputState)
        {

            if (Vector2.Distance(new Vector2(_Position.X, _Position.Z), _currentTile.getDrawPos()) <= MIN_MOVE_DIS)
            {
                // Only allow movement if the player is on the next tile
                moveCharacter(inputState);
            }

            _Position.X += MathUtils.smoothChange(_Position.X, _currentTile.getDrawPos().X, MOVE_RATE);
            _Position.Z += MathUtils.smoothChange(_Position.Z, _currentTile.getDrawPos().Y, MOVE_RATE);

           

            if (ArenaScene.instance.collidables != null && ArenaScene.instance.collidables.Count != 0)//make sure the list isn't empty to check the collisions
            {
                //a placeholder to loop through so removing items wouldn't result in IndexOutOfBounds if the list was used to loop through
                Entity[] collidablesArray = ArenaScene.instance.collidables.ToArray();

                for (int i = 0; i < collidablesArray.Length; i++)
                {
                    if (collidablesArray[i].GetType() == typeof(ArenaEnemy))//if the collideable is an enemy
                    {
                        if (PhysicsUtils.CheckCollision(this, (ArenaEnemy)collidablesArray[i]))
                        {
                            // TEMP: Kill the enemy
                            ((ArenaEnemy)collidablesArray[i]).die();
                        }
                    }
                    else if (ArenaScene.instance.collidables[i].GetType() == typeof(ArenaExit))//if the collideable is the door
                    {
                        if (PhysicsUtils.CheckCollision(this, (ArenaExit)collidablesArray[i]))
                        {
                            // Continue to the next arena
                            ArenaController.instance.moveToNextArena();
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Method to get the characters current position.
        /// </summary>
        /// <returns>The position of the player character as a Vector3.</returns>
        public Vector3 getPosition()
        {
            return _Position;
        }

        /// <summary>
        /// the position the character will end at.
        /// </summary>
        /// <returns></returns>
        public Vector3 getNextPosition()
        {
            return _currentTile.getModelPos();
        }

        /// <summary>
        /// will take the input states from the update method call.
        /// If any of the states indicate character movement, the characters _positon will be updated.
        /// </summary>
        /// <param name="baseArena"></param>
        private void moveCharacter(InputState inputState)
        {
            PlayerIndex player;
            if (up.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.TOP) != null)
                {
                    //deltaZ = deltaZ - Tile.TILE_HEIGHT;
                    Tile temp = _currentTile.getConnection(TileConnections.TOP);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();
                }

                rotAngle = MathHelper.ToRadians(180);
            }

            if (down.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.BOTTOM) != null)
                {
                    //deltaZ = deltaZ + Tile.TILE_HEIGHT;
                    Tile temp = _currentTile.getConnection(TileConnections.BOTTOM);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();
                }

                rotAngle = MathHelper.ToRadians(0);
            }

            
            else if (left.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.LEFT) != null)
                {
                    //deltaX = deltaX - Tile.TILE_WIDTH;
                    Tile temp = _currentTile.getConnection(TileConnections.LEFT);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();
                }

                rotAngle = MathHelper.ToRadians(270);
            }

            else if (right.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.RIGHT) != null)
                {
                    //deltaX = deltaX + Tile.TILE_WIDTH;
                    Tile temp = _currentTile.getConnection(TileConnections.RIGHT);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();
                }

                rotAngle = MathHelper.ToRadians(90);
            }

            //update model rotation
            modelRotation = rotAngle;           
        }


    }
}
