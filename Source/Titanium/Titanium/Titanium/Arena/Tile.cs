using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Titanium.Entities;
using Titanium.Scenes;

namespace Titanium.Arena
{
    /// <summary>
    /// This enum contains all possible connection directions to another tile.
    /// </summary>
    public enum TileConnections
    {
        LEFT,
        TOP,
        RIGHT,
        BOTTOM,
        NONE
    }

    /// <summary>
    /// This class represents a single tile in the arena.
    /// </summary>
    public class Tile : Entity
    {
        public static int TILE_WIDTH = 128;
        public static int TILE_HEIGHT = 128;
        public static float tileScale = 0.5f;
        public static float UNDISCOVERED_AMBIENCE = 0.045f;
        public static float DISCOVERED_AMBIENCE = 0.8f;

        private ArenaTiles tile;
        private Texture2D texture;
        private Model model;
        //MovableModel
        public Matrix ModelMatrix;
        private Vector3 modelPosition;

        public float TileAmbience;


        private Vector2 pos;
        private Vector2 drawPos;

        private Tile[] connections = new Tile[4];
        private Tile parent;

        private List<Entity> entityList = new List<Entity>();
        private List<Entity> queuedRemove = new List<Entity>();
        
        /// <summary>
        /// The base Tile constructor. This is responsible for loading the tile art and setting its position on the 'board'.
        /// </summary>
        /// <param name="parent">The tile's parent (creator)</param>
        /// <param name="xPos">The x position of the tile in the array</param>
        /// <param name="yPos">The y position of the tile in the array</param>
        public Tile(Tile parent, int xPos, int yPos)
        {
            this.parent = parent;      

            // Set the position in the array
            pos = new Vector2(xPos, yPos);

            // Set the world position based on the position in the array
            drawPos = new Vector2(xPos * TILE_WIDTH, yPos * TILE_HEIGHT);

            // Determine the location on-screen compared to the start tile
            Tile startTile = ArenaBuilder.instance.getStartTile();
                        
            modelPosition = new Vector3(drawPos.X, -10, drawPos.Y);

            //set tile ambience as arena ambience as default
            TileAmbience = UNDISCOVERED_AMBIENCE;
        }

        /// <summary>
        /// lights up the tile if it's been walked on
        /// </summary>
        public void setWalkedOnAmbience()
        {
            TileAmbience = DISCOVERED_AMBIENCE;
        }

        /// <summary>
        /// returns true if the ambience is set
        /// to discovered (a tile that was already walked on),
        /// and false if it has not been discovered.
        /// </summary>
        /// <returns>true or flase depending on if the tile was discovered</returns>
        public bool walkedOn()
        {
            if (TileAmbience != DISCOVERED_AMBIENCE)
                return false;
            else
                return true;
        }

        /// <summary>
        /// This function updates the tile.
        /// </summary>
        /// <param name="gameTime">The gametime object for timing</param>
        /// <param name="inputState">The input state object for input</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            // Update each entity
            foreach (Entity entity in entityList)
            {
                entity.Update(gameTime, inputState);
            }

            // Clear out any entities deleted in this turn
            foreach (Entity entity in queuedRemove)
            {
                entityList.Remove(entity);
            }

            queuedRemove.Clear();
        }

        /// <summary>
        /// This function renders the tile to the screen
        /// </summary>
        /// <param name="sb">The sprite batch object used for rendering</param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {

            if (model != null)//don't do anything if the model is null
            {
                // Copy any parent transforms.
                Matrix worldMatrix = Matrix.CreateRotationY(-1.5708f) * Matrix.CreateScale(tileScale) * Matrix.CreateTranslation(modelPosition);

                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well as our camera and projection.
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {

                        foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                        {
                            part.Effect = effect;

                            effect.Parameters["AmbientIntensity"].SetValue(TileAmbience);
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

            foreach (Entity entity in entityList)
            {
                entity.Draw(sb, effect);
            }
        }

        /// <summary>
        /// This function sets the tile's graphical component
        /// </summary>
        /// <param name="tile">The tile type</param>
        /// <param name="Content">The content manager for loading the art</param>
        public void setArenaTile(ArenaTiles tile, string type, ContentManager Content)
        {
            this.tile = tile;

            // Load the UV Map and model
            texture = Content.Load<Texture2D>("Models/" + type + "/UVMap" + (int) tile);
            model = Content.Load<Model>("Models/tileBase");
        }

        /// <summary>
        /// This function sets a connection between this tile and another.
        /// </summary>
        /// <param name="dir">The connection direction</param>
        /// <param name="t">The tile to connect with</param>
        public void setConnection(TileConnections dir, Tile t)
        {
            connections[(int) dir] = t;
        }

        /// <summary>
        /// This function returns a connection between this tile and another.
        /// </summary>
        /// <param name="dir">The connection direction</param>
        /// <returns>The tile object if the connection exists, otherwise null</returns>
        public Tile getConnection(TileConnections dir)
        {
            return connections[(int) dir];
        }

        /// <summary>
        /// This function returns the tile's connections.
        /// </summary>
        /// <returns>The tile's connections</returns>
        public Tile[] getConnections()
        {
            return connections;
        }


        /// <summary>
        /// This function returns the number of connections that the tile has.
        /// </summary>
        /// <returns>The number of connections</returns>
        public int getNumConnections()
        {
            int numConnections = 0;

            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i] != null)
                {
                    numConnections++;
                }
            }

            return numConnections;
        }

        /// <summary>
        /// This function adds an entity to the tile.
        /// </summary>
        /// <param name="entity">The entity to add</param>
        public void addEntity(Entity entity)
        {
            entityList.Add(entity);
        }

        /// <summary>
        /// This function returns the list of entities currently on this tile.
        /// </summary>
        /// <returns>The list of entities on the tile.</returns>
        public List<Entity> getEntities()
        {
            return entityList;
        }

        /// <summary>
        /// This function deletes an entity from the list.
        /// </summary>
        /// <param name="e">The entity to delete.</param>
        public void deleteEntity(Entity e)
        {
            queuedRemove.Add(e);
        }

        /// <summary>
        /// This function returns the logical position of the tile in the array.
        /// </summary>
        /// <returns>The position of the tile</returns>
        public Vector2 getPos()
        {
            return pos;
        }

        /// <summary>
        /// This function returns the visual position of the tile.
        /// </summary>
        /// <returns>The visual position of the tile</returns>
        public Vector2 getDrawPos()
        {
            return drawPos;
        }

        /// <summary>
        /// This function returns the position of the model on-screen
        /// </summary>
        /// <returns>The model position</returns>
        public Vector3 getModelPos()
        {
            return modelPosition;
        }

        /// <summary>
        /// This function returns the type of the tile.
        /// </summary>
        /// <returns>The tile type</returns>
        public ArenaTiles getType()
        {
            return tile;
        }

        public bool hasEnemy()
        {
            bool hasEnemy = false;

            foreach (Entity e in entityList)
            {
                if ( e.GetType() == typeof(ArenaEnemy))
                {
                    hasEnemy = true;
                    break;
                }
            }

            return hasEnemy;
        }

        public bool hasExit()
        {
            bool hasExit = false;

            foreach (Entity e in entityList)
            {
                if (e.GetType() == typeof(ArenaExit))
                {
                    hasExit = true;
                    break;
                }
            }

            return hasExit;
        }

        /// <summary>
        /// returns modelPosition;
        /// does the same as method getModelPositon()
        /// </summary>
        /// <returns>model's position</returns>
        public override Vector3 getPOSITION()
        {
            return modelPosition;
        }
    }
}
