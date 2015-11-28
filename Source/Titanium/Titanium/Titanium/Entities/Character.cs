using System;
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
using Titanium.Entities.Items;

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
        //public Model myModel;
        //public Matrix ModelMatrix;
        //private SpriteBatch spriteBatch;
        //private String modelPath;
        private Vector3 modelPosition;
        private int stepsTaken;

        private Texture2D texture;
        
        private float rotAngle;
        private float scale;

        // Possible player actions
        static InputAction up, down, left, right, zoom;

        static Character()
        {
            up = InputAction.UP;
            down = InputAction.DOWN;
            left = InputAction.LEFT;
            right = InputAction.RIGHT;
            zoom = InputAction.RCLICK;
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

            rotAngle = MathHelper.ToRadians(180);
            scale = 0.5f;

            modelRotation = rotAngle;

            myModel = null;
            stepsTaken = 0;
        }

        public void LoadModel(ContentManager cm, float aspectRatio)
        {
            myModel = cm.Load<Model>("Models/hero");
            texture = cm.Load<Texture2D>("Models/PlayerMap");
            this.aspectRatio = aspectRatio;
        }

        /// <summary>
        /// inherited draw method from Entity class.
        /// </summary>
        /// <param name="sb"></param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (myModel != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateScale(scale, scale, scale) * Matrix.CreateRotationY(modelRotation) * Matrix.CreateTranslation(_Position);

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
                            effect.Parameters["ModelTexture"].SetValue(texture);

                            Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * worldMatrix));
                        }
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

            checkCharacterCollisions();


        }

        /// <summary>
        /// Detects and handles collision between the character
        /// and other entities in the level.
        /// </summary>
        private void checkCharacterCollisions()
        {

            if (ArenaScene.instance.collidables != null && ArenaScene.instance.collidables.Count != 0)//make sure the list isn't empty to check the collisions
            {
                foreach (Entity e in ArenaScene.instance.collidables.ToList())
                {
                    if (PhysicsUtils.CheckCollision(this, e))
                    {
                        switch (e.GetType().Name)
                        {
                            case "ArenaEnemy":
                                // TEMP: Kill the enemy
                                ((ArenaEnemy) e).die();
                                ArenaScene.instance.startBattle(((ArenaEnemy) e).getEnemyType());

                                // Snap to the target tile
                                _Position.X = _currentTile.getDrawPos().X;
                                _Position.Z = _currentTile.getDrawPos().Y;
                                break;

                            case "ArenaExit":
                                // Continue to the next 
                                ArenaController.instance.moveToNextArena();
                                break;

                            case "Potion":
                                Potion p = (Potion)e;
                                //heal party members a certain precentage
                                PartyUtils.HealParty(p.getHealPercent());
                                ArenaScene.instance.potionsUsed++;

                                //remove from tile and list
                                p.getTile().deleteEntity(e);
                                ArenaScene.instance.collidables.Remove(e);

                                // play a sound
                                SoundUtils.Play(SoundUtils.Sound.Potion);
                                break;

                            case "MysteryBox":
                                MysteryBox mb = (MysteryBox)e;
                                if (!mb.getCollected())
                                {
                                    mb.RevealMystery();
                                    mb.setCollected();
                                }
                                break;
                        }
                    }

   
                }
            }
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
            if(!_currentTile.walkedOn())
                _currentTile.setWalkedOnAmbience();


            PlayerIndex player;
            
            if (up.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.TOP) != null)
                {
                    //deltaZ = deltaZ - Tile.TILE_HEIGHT;
                    Tile temp = _currentTile.getConnection(TileConnections.TOP);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();

                    SoundUtils.Play(SoundUtils.Sound.Step);

                   stepsTaken++;
                }
            }

            else if (down.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.BOTTOM) != null)
                {
                    //deltaZ = deltaZ + Tile.TILE_HEIGHT;
                    Tile temp = _currentTile.getConnection(TileConnections.BOTTOM);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();

                    SoundUtils.Play(SoundUtils.Sound.Step);

                    stepsTaken++;
                }
            }

            
            else if (left.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.LEFT) != null)
                {
                    //deltaX = deltaX - Tile.TILE_WIDTH;
                    Tile temp = _currentTile.getConnection(TileConnections.LEFT);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();
                    SoundUtils.Play(SoundUtils.Sound.Step);
                    stepsTaken++;
                }

                rotAngle = MathHelper.ToRadians(0);
            }

            else if (right.Evaluate(inputState, PlayerIndex.One, out player))
            {
                if (_currentTile.getConnection(TileConnections.RIGHT) != null)
                {
                    //deltaX = deltaX + Tile.TILE_WIDTH;
                    Tile temp = _currentTile.getConnection(TileConnections.RIGHT);
                    _currentTile = temp;

                    ArenaController.instance.setMoved();

                    SoundUtils.Play(SoundUtils.Sound.Step);

                    stepsTaken++;
                }

                rotAngle = MathHelper.ToRadians(180);
            }

            //update model rotation
            modelRotation = rotAngle;           
        }

        /// <summary>
        /// gets the number of steps the character has taken
        /// </summary>
        /// <returns></returns>
        public int getSteps()
        {
            return stepsTaken;
        }


        /// <summary>
        /// resets the amount of steps taken by the character to 0
        /// </summary>
        public void resetSteps()
        {
            stepsTaken = 0;
        }


    }
}
