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
    class Roulette: BaseGambit
    {
        Texture2D[] icons;
        public static string[] iconDirs = { "arrow-up", "arrow-left", "arrow-down", "arrow-right" };
        List<RouletteWheel> wheels;

        int speed, wheelNum, iconNum;

        public Roulette()
        {
            name = "Roulette";
            message = "Match the icons with:";
            helpOffset -= new Vector2(0, 50);
        }


        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/MainFontSmall");

            List<Texture2D> icons = new List<Texture2D>();
            foreach (string dir in iconDirs)
                icons.Add(content.Load<Texture2D>("ButtonIcons/" + dir));

            this.icons = icons.ToArray();

        }
        public override void start(GameTime gameTime, int difficulty)
        {
            wheels = new List<RouletteWheel>();
            switch((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    speed = 3;
                    wheelNum = 3;
                    iconNum = 3;
                    break;
                case Difficulty.Medium:
                    speed = 3;
                    wheelNum = 4;
                    iconNum = 4;
                    break;
                case Difficulty.Hard:
                    speed = 4;
                    wheelNum = 4;
                    iconNum = 4;
                    break;
                default:
                    speed = 3;
                    wheelNum = 3;
                    break;
            }
            for(int i=0; i<wheelNum; i++)
            {
                wheels.Add(new RouletteWheel(getIconList(iconNum), speed));
            }
            base.start(gameTime, difficulty);
        }

        List<Texture2D> getIconList(int num)
        {
            List<Texture2D> iconList = new List<Texture2D>();
            for(int i=0; i<icons.Length; ++i)
            {
                if (num == 0)
                    break;
                iconList.Add(icons[i]);
                num--;
            }
            if(iconList.Count == 2)
            {
                iconList = iconList.Concat(iconList).ToList();
            }
            else if (iconList.Count == 3)
            {
                iconList.Add(iconList[new Random().Next(iconList.Count)]);
            }
            return iconList;
            
        }

        public override void update(GameTime gameTime, InputState state)
        {
            bool done = true;
            foreach (RouletteWheel wheel in wheels)
            {
                wheel.update(gameTime, state);
                done = !wheel.spinning && done;
            }

            if(done)
            {
                finished = true;
                multiplier = getMultiplier();
            } 

            if(InputAction.A.wasPressed(state))
            {
                foreach (RouletteWheel wheel in wheels)
                {
                    if(wheel.spinning && !wheel.pendingStop)
                    {
                        SoundUtils.Play(SoundUtils.Sound.Success);
                        wheel.stop();
                        break;
                    }
                }
            }   
            
            base.update(gameTime, state);
        }

        float getMultiplier()
        {
            float result = 0f;
            List<Texture2D> selections = new List<Texture2D>();
            foreach(RouletteWheel wheel in wheels)
            {
                selections.Add(wheel.selected());
            }
            result = (float)getMaxMatches(selections) / iconNum;
            return result;
        }

        int getMaxMatches(List<Texture2D> list)
        {
            int[] matches = new int[list.Count];
            for(int i=0; i<list.Count; ++i)
            {
                matches[i] = 0;
                foreach (Texture2D item in list)
                {
                    if(list[i].Equals(item))
                    {
                        matches[i]++;
                    }
                }
            }
            int max = matches.Max();
            if(max == wheelNum)
            {
                SoundUtils.Play(SoundUtils.Sound.Complete);
            }
            else
            {
                SoundUtils.Play(SoundUtils.Sound.Failure);
            }
            return max;
        }

        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            foreach (RouletteWheel wheel in wheels)
            {
                wheel.draw(pos, sb);
                pos += new Vector2(RouletteWheel.width+25, 0);
            }

            sb.Draw(InputAction.A.icon(), helpOffset + pos + new Vector2(font.MeasureString(message).X/2 - 30, font.LineSpacing+5), Color.White);
            base.draw(pos, sb);
        }

        public override int totalHeight()
        {
            throw new NotImplementedException();
        }

        public override int totalWidth()
        {
            throw new NotImplementedException();
        }

        class RouletteWheel
        {
            public static int width = 75;
            static int height = 175;
            static int iconWidth = 50;
            static int iconHeight = 50;
            static int iconPadding = 8;

            public bool spinning = true;
            List<Texture2D> icons;
            Rectangle[] iconPositions;
            Texture2D pixel;
            Vector2 pos;


            Rectangle leftLine, rightLine, centerLine;
            Rectangle topBuffer, bottomBuffer;
            Rectangle background;

            int speed = 4;
            bool initialized = false;
            public bool pendingStop = false;

            public RouletteWheel(List<Texture2D> icons, int speed)
            {
                this.icons = icons;
                leftLine = new Rectangle();
                rightLine = new Rectangle();
                centerLine = new Rectangle();
                topBuffer = new Rectangle();
                bottomBuffer = new Rectangle();
                background = new Rectangle();
                iconPositions = new Rectangle[icons.Count];
                this.speed = speed;
            }

            public void update(GameTime gameTime, InputState inputState)
            {
                if(pos != Vector2.Zero && !initialized)
                {
                    background = new Rectangle((int)pos.X, (int)pos.Y, width, height);
                    leftLine = new Rectangle((int)pos.X, (int)pos.Y, 5, height);
                    rightLine = new Rectangle((int)pos.X + width-5, (int)pos.Y, 5, height);
                    centerLine = new Rectangle((int)pos.X, (int)pos.Y + height / 2, width, 3);
                    topBuffer = new Rectangle((int)pos.X, (int)pos.Y - iconPadding, width, iconHeight+iconPadding);
                    bottomBuffer = new Rectangle((int)pos.X, (int)pos.Y + height - iconHeight, width, iconHeight+iconPadding);
                    for (int i = 0; i < iconPositions.Length; ++i)
                    {
                        iconPositions[i] = new Rectangle((int)pos.X + 12, (int)pos.Y + ((iconHeight+iconPadding) * i), iconWidth, iconHeight);
                    }
                    initialized = true;   
                }
                if(initialized && spinning)
                {
                    if (!pendingStop)
                        advancePositions();
                    else
                        advancePositionsSlow();
                }

            }

            public void stop()
            {
                pendingStop = true;
            }

            void advancePositions()
            {
                for (int i = 0; i < iconPositions.Length; ++i)
                {
                    if (iconPositions[i].Top > bottomBuffer.Top)
                        iconPositions[i] = new Rectangle(iconPositions[i].Left, topBuffer.Top, iconWidth, iconHeight);
                    else if(shouldMove(i))
                        iconPositions[i] = new Rectangle(iconPositions[i].Left, iconPositions[i].Top + speed, iconWidth, iconHeight);
                }
                
            }

            public Texture2D selected()
            {
                for(int i=0; i<iconPositions.Length; ++i)
                {
                    if(iconPositions[i].Intersects(centerLine))
                    {
                        return icons[i];
                    }
                }
                return null;
            }

            void advancePositionsSlow()
            {
                for (int i = 0; i < iconPositions.Length; ++i)
                {
                    if (iconPositions[i].Top > bottomBuffer.Top)
                        iconPositions[i] = new Rectangle(iconPositions[i].Left, topBuffer.Top, iconWidth, iconHeight);
                    else if (shouldMove(i))
                        iconPositions[i] = new Rectangle(iconPositions[i].Left, iconPositions[i].Top + 1, iconWidth, iconHeight);
                    if (iconPositions[i].Center.Y == centerLine.Center.Y)
                        spinning = false;
                }

            }

            bool shouldMove(int i)
            {
                int j = 0;
                if (i < iconPositions.Length - 1)
                    j = i + 1;

                //return (!iconPositions[j].Intersects(iconPositions[i]));
                return (!iconPositions[i].Intersects(topBuffer) || iconPositions[j].Top - iconPositions[i].Bottom >= iconPadding+1);
            }

            public void draw(Vector2 pos, SpriteBatch sb)
            {
                if (pixel == null)
                {
                    pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    pixel.SetData(new Color[] { Color.White });
                }

                if (this.pos == Vector2.Zero)
                    this.pos = pos;
                else
                {
                    sb.Draw(pixel, background, Color.White);
                    sb.Draw(pixel, leftLine, Color.Black);
                    sb.Draw(pixel, rightLine, Color.Black);
                    for (int i = 0; i < icons.Count; i++)
                    {
                        sb.Draw(icons[i], iconPositions[i], Color.White);
                    }
                    sb.Draw(pixel, centerLine, Color.ForestGreen);
                    sb.Draw(pixel, topBuffer, Color.Black);
                    sb.Draw(pixel, bottomBuffer, Color.Black);

                }
                
            }

        }
    }
}
