using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Titanium.Gambits
{
    class Rhythm : BaseGambit
    {

        public static int width = 550;
        public static int height = 240;
        public static int iconWidth = 60;
        public static int bufferWidth = 70;
        public static int speed = 2;

        enum Direction { up, left, down, right }

        InputAction[] actions =
        {
            new InputAction(
                new Buttons[] {Buttons.DPadUp, Buttons.Y },
                new Keys[] { },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadLeft, Buttons.X },
                new Keys[] { },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadDown, Buttons.A },
                new Keys[] { },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadRight, Buttons.B },
                new Keys[] { },
                true
                )
        };

        
        Rectangle leftLine, rightLine;
        Texture2D pixel;
        Vector2 position;
        List<RhythmInput> rhythmString;

        public Rhythm()
        {

        }

        public override void draw(Vector2 pos, SpriteBatch sb)            
        {
            if(pixel == null)
            {
                pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new Color[] { Color.White });
            }
            if (position == null)
            {
                position = pos;
                setLines();
            }
                

            sb.Draw(pixel, leftLine, Color.Black);
            sb.Draw(pixel, rightLine, Color.Black);

        }

        public override void update(GameTime gameTime, InputState state)
        {
            base.update(gameTime, state);
            position += new Vector2(speed, 0);
            setLines();
            
        }

        public void setLines()
        {
            leftLine = new Rectangle((int)position.X, (int)position.Y, 2, height);
            rightLine = new Rectangle((int)position.X+bufferWidth, (int)position.Y, 2, height);
        }

        public override void load(ContentManager content)
        {
            foreach(RhythmInput input in rhythmString)
            {
                input.load(content);
            }
        }

        public override int totalHeight()
        {
            throw new NotImplementedException();
        }

        public override int totalWidth()
        {
            throw new NotImplementedException();
        }

        class RhythmInput
        {
            static string[] iconDirs = { "arrow-up", "arrow-left", "arrow-down", "arrow-right" };

            enum Result { perfect, fair, miss, pending };

            Result result;
            Direction direction;
            Vector2 offset;
            Rectangle position;
            Texture2D icon;
            InputAction action;
            Color color;


            public RhythmInput(float offset, Vector2 size, Direction dir)
            {
                direction = dir;
                this.offset = new Vector2(offset, 0.25f * (int)dir) * size;
                result = Result.pending;
                setColor();
            }

            public void load(ContentManager content)
            {
                icon = content.Load<Texture2D>("ButtonIcons/" + iconDirs[(int)direction]);
            }

            public void setColor()
            {
                switch (result)
                {
                    case Result.perfect:
                        color = Color.Green;
                        break;
                    case Result.fair:
                        color = Color.Yellow;
                        break;
                    case Result.miss:
                        color = Color.Red;
                        break;
                    default:
                        color = Color.White;
                        break;
                }
            }

            public void checkResult(int left, int right)
            {
                if (position.Left > left && position.Right < right)
                    result = Result.perfect;
                else if (position.Left < left && position.Right > left)
                    result = Result.fair;
                else if (position.Left < right && position.Right > right)
                    result = Result.fair;
                else if (position.Right < left)
                    result = Result.miss;

                setColor();
            }

            public void draw(Vector2 pos, SpriteBatch sb)
            {
                if(position == null)
                {
                    Vector2 v = pos + offset;
                    position = new Rectangle((int)v.X, (int)v.Y, iconWidth, iconWidth);
                }

                sb.Draw(icon, position, color);
            }



        }
    }
}
