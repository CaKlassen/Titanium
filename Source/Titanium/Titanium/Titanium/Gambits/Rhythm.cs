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
        public static int height = 200;
        public static int iconWidth = 50;
        public static int bufferWidth = 65;
        public static int inputOffset = 50;
        public static int speed = 2;
        public static string[] iconDirs = { "arrow-up", "arrow-left", "arrow-down", "arrow-right" };

        enum Direction { up, left, down, right }

        static InputAction[] actions =
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
        Random rng;
        List<Texture2D> icons;

        public Rhythm()
        {
            rhythmString = new List<RhythmInput>();
        }

        public override void draw(Vector2 pos, SpriteBatch sb)            
        {
            if(pixel == null)
            {
                pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new Color[] { Color.White });
            }
            if (position.Y == 0)
            {
                position = pos;
                setLines();
            }
            
                

            sb.Draw(pixel, leftLine, Color.Black);
            sb.Draw(pixel, rightLine, Color.Black);

            foreach (RhythmInput input in rhythmString)
                input.draw(pos+new Vector2(inputOffset,0), sb);
        }

        public override void update(GameTime gameTime, InputState state)
        {
            base.update(gameTime, state);
            position += new Vector2(speed, 0);
            setLines();

            foreach(RhythmInput input in rhythmString)
            {
                if (input.inRange(leftLine.Right, rightLine.Left))
                { 
                    if (input.action.wasPressed(state))
                    {
                        input.checkResult(leftLine.Right, rightLine.Left);
                    }
                        
                }
            }
            if (leftLine.Left > 1000)
            {
                multiplier = getMultiplier();
                finished = true;
            }
            
        }

        public float getMultiplier()
        {
            float multiplier = 0.7f;
            foreach(RhythmInput input in rhythmString)
            {
                switch(input.result)
                {
                    case RhythmInput.Result.fair:
                        multiplier += 0.05f;
                        break;
                    case RhythmInput.Result.perfect:
                        multiplier += 0.1f;
                        break;
                }
            }
            return multiplier;
        }

        public override void start(GameTime gameTime)
        {
            base.start(gameTime);
            rng = new Random(gameTime.TotalGameTime.Milliseconds);
            rhythmString = makeRhythmString();
            leftLine = new Rectangle();
            rightLine = new Rectangle();
            position = Vector2.Zero;
        }

        List<RhythmInput> makeRhythmString()
        {
            List<RhythmInput> list = new List<RhythmInput>();
            for(int i=0; i<4; ++i)
            {
                for(int j=0; j<2; ++j)
                    list.Add(new RhythmInput((float)rng.NextDouble(), new Vector2(width-inputOffset, height), (Direction)i, icons));
            }

            list.Sort(delegate (RhythmInput a, RhythmInput b)
            {
                if (a.Offset < b.Offset) return -1;
                if (a.Offset > b.Offset) return 1;
                return 0;
            });
            return list;
        }

        public void setLines()
        {
            leftLine = new Rectangle((int)position.X, (int)position.Y, 2, height);
            rightLine = new Rectangle((int)position.X+bufferWidth, (int)position.Y, 2, height);
        }

        public override void load(ContentManager content)
        {
            icons = new List<Texture2D>();
            foreach(string dir in iconDirs)
                icons.Add(content.Load<Texture2D>("ButtonIcons/" + dir));
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
            

            public enum Result { perfect, fair, miss, pending };
            public Result result;

            Direction direction;
            Vector2 offset;
            Rectangle position;
            Texture2D icon;
            public InputAction action;
            Color color;
            public float Offset;

            public RhythmInput(float offset, Vector2 size, Direction dir, List<Texture2D> icons)
            {
                Offset = offset;
                direction = dir;
                this.offset = new Vector2(offset, 0.25f * (int)dir) * size;
                result = Result.pending;
                action = actions[(int)dir];
                setColor();
                icon = icons[(int)direction];
            }

            public void setColor()
            {
                switch (result)
                {
                    case Result.perfect:
                        Console.WriteLine("PERFECT");
                        break;
                    case Result.fair:
                        Console.WriteLine("FAIR");
                        break;
                    case Result.miss:
                        Console.WriteLine("MISS");
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
                else if (position.Right < left || position.Left > right)
                    result = Result.miss;

                setColor();
            }

            public bool inRange(int left, int right)
            {
                if (position.Width < 1)
                    return false;
                if (result != Result.pending)
                    return false;
                else if (position.Right < left)
                {
                    result = Result.miss;
                    setColor();
                    return false;
                }
                else if (position.Left - right < inputOffset)
                {
                    return true;
                }
                return false;
            }

            public void draw(Vector2 pos, SpriteBatch sb)
            {
                if(position.Width == 0)
                {
                    Vector2 v = pos + offset;
                    position = new Rectangle((int)v.X, (int)v.Y, iconWidth, iconWidth);
                }
                sb.Draw(icon, position, color);
            }



        }
    }
}
