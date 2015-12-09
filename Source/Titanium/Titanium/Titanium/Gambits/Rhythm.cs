using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Utilities;

namespace Titanium.Gambits
{
    class Rhythm : BaseGambit
    {

        public static int width = 550;
        public static int height = 200;
        public static int iconWidth = 50;
        public static int bufferWidth = 65;
        public static int inputOffset = 70;
        public static float speed = 3f;
        static int directions = 4;
        public static string[] iconDirs = { "arrow-up", "arrow-left", "arrow-down", "arrow-right" };

        enum Direction { up, left, down, right }

        static InputAction[] actions =
        {
            new InputAction(
                new Buttons[] {Buttons.DPadUp, Buttons.Y },
                new Keys[] { Keys.Up },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadLeft, Buttons.X },
                new Keys[] { Keys.Left },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadDown, Buttons.A },
                new Keys[] { Keys.Down },
                true
                ),
            new InputAction(
                new Buttons[] {Buttons.DPadRight, Buttons.B },
                new Keys[] { Keys.Right },
                true
                )
        };

        
        Rectangle leftLine, rightLine;
        Texture2D pixel;
        Vector2 position;
        List<RhythmInput> rhythmString;
        Random rng;
        List<Texture2D> icons;
        int startDelay = 2000;
        int inputs = 2;
        
        float multStep;
        float fairFactor;

        public Rhythm()
        {
            rhythmString = new List<RhythmInput>();
            name = "Rhythm";
        }

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    inputs = 1;
                    break;
                case Difficulty.Medium:
                    inputs = 2;
                    break;
                case Difficulty.Hard:
                    inputs = 3;
                    break;
                default:
                    inputs = 2;
                    break;

            }
            fairFactor = 1f;
            multStep = 1f / (inputs * directions);
            rng = new Random(gameTime.TotalGameTime.Milliseconds);
            rhythmString = makeRhythmString();
            leftLine = new Rectangle();
            rightLine = new Rectangle();
            position = Vector2.Zero;
            startDelay = 2000;
            setLines();
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

            drawHelpIcons(pos, sb);
        }

        public void drawHelpIcons(Vector2 pos, SpriteBatch sb)
        {
            Rectangle first, second;
            for(int i=0; i<actions.Length; ++i)
            {
                first = new Rectangle((int)pos.X-10, (int)pos.Y + (i * 50), 30, 30);
                second = new Rectangle(first.X + 20, first.Y + 20, 30, 30);
                sb.Draw(actions[i].icon(0), first, Color.White);
                sb.Draw(actions[i].icon(1), second, Color.White);
            }
                
        }

        public override void update(GameTime gameTime, InputState state)
        {
            

            if (startDelay > 0)
                startDelay-= gameTime.ElapsedGameTime.Milliseconds;
            else
            {
                base.update(gameTime, state);
                position += new Vector2(speed, 0);
                setLines();
                foreach (RhythmInput input in rhythmString)
                {
                    if (input.inRange(leftLine, rightLine))
                    {
                        if (input.action.wasPressed(state))
                        {
                            input.checkResult(leftLine.Right, rightLine.Left);
                            break;
                        }

                    }
                }
                if (leftLine.Left > 1000)
                {
                    multiplier = getMultiplier();
                    finished = true;
                }
            }
            
            
        }

        public float getMultiplier()
        {
            float multiplier = 0f;
            foreach(RhythmInput input in rhythmString)
            {
                switch(input.result)
                {
                    case RhythmInput.Result.fair:
                        multiplier += multStep/fairFactor;
                        break;
                    case RhythmInput.Result.perfect:
                        multiplier += multStep;
                        break;
                }
            }
            return multiplier;
        }


        List<RhythmInput> makeRhythmString()
        {
            List<RhythmInput> list = new List<RhythmInput>();
            for(int i=0; i<4; ++i)
            {
                for(int j=0; j<inputs; ++j)
                {
                    double num;
                    do
                    {
                        num = rng.NextDouble();
                    } while (tooClose(num, list));
                    list.Add(new RhythmInput((float)num, new Vector2(width - inputOffset, height), (Direction)i, icons));
                }
                    
            }

            list.Sort(delegate (RhythmInput a, RhythmInput b)
            {
                if (a.Offset < b.Offset) return -1;
                if (a.Offset > b.Offset) return 1;
                return 0;
            });
            return list;
        }

        bool tooClose(double num, List<RhythmInput> list)
        {
            foreach(RhythmInput input in list)
            {
                if (Math.Abs((float)num - input.Offset) < 0.02)
                    return true;
            }
            return false;
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

            font = content.Load<SpriteFont>("Fonts/NumbersFont");
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
            static int wiggle = 8;

            public enum Result { perfect, fair, miss, pending };
            public Result result;

            public Direction direction;
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
                        SoundUtils.Play(SoundUtils.Sound.Success);
                        color = Color.Transparent;//Color.Green;
                        break;
                    case Result.fair:
                        Console.WriteLine("FAIR");
                        SoundUtils.Play(SoundUtils.Sound.Input);
                        color = Color.Transparent;
                        break;
                    case Result.miss:
                        Console.WriteLine("MISS");
                        SoundUtils.Play(SoundUtils.Sound.Failure);
                        color = Color.Black;
                        break;
                    default:
                        color = Color.White;
                        break;
                }
            }

            public void checkResult(int left, int right)
            {
                left -= wiggle;
                right += wiggle;
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

            public bool inRange(Rectangle left, Rectangle right)
            {
                if (result != Result.pending)
                    return false;
                if (position.Right < left.Left-wiggle)
                {
                    result = Result.miss;
                    setColor();
                    return false;
                }
                if (left.Intersects(position) || right.Intersects(position) || ( position.Left >= left.Left && position.Right <= right.Right) )
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
