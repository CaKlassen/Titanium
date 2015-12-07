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
    class Counter : BaseGambit
    {

        static int trackWidth = 400;
        static int trackHeight = 15;
        static int lineHeight = 80;
        static int trackOffsetX = 90;
        static int trackOffsetY = 80;
        static string[] helpText = { "Hit", "at the right time!" };


        enum Direction { up, left, down, right }

        Rectangle line;
        Rectangle track;
        Rectangle perfect;
        Rectangle fair;

        Rectangle icon;

        Texture2D pixel;


        float speed = 5f;
        int perfectWidth;        
        int fairWidth;


        Vector2 position;

        
        int startDelay = 500;

        public Counter()
        {
            name = "";
            helpOffset = new Vector2(530, 180);
        }

        public override void start(GameTime gameTime, int difficulty)
        {
            base.start(gameTime, difficulty);
            switch ((Difficulty)difficulty)
            {
                case Difficulty.Easy:
                    perfectWidth = 20;
                    speed = 5f;
                    break;
                case Difficulty.Medium:
                    perfectWidth = 17;
                    speed = 6f;
                    break;
                case Difficulty.Hard:
                    perfectWidth = 13;
                    speed = 7f;
                    break;
                default:
                    perfectWidth = 20;
                    speed = 5f;
                    break;

            }
            fairWidth = perfectWidth * 3;
            line = new Rectangle();
            track = new Rectangle();
            perfect = new Rectangle();
            fair = new Rectangle();
            position = Vector2.Zero;
            startDelay = 500;
            setLine();
        }


        public override void draw(Vector2 pos, SpriteBatch sb)
        {
            if (pixel == null)
            {
                pixel = new Texture2D(sb.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                pixel.SetData(new Color[] { Color.White });
            }
            if (position.Y == 0)
            {
                position = pos;
                setTrack();
                setLine();
            }

            drawMessage(helpOffset, sb);
            sb.Draw(pixel, track, Color.Black);
            sb.Draw(pixel, fair, Color.Gold);
            sb.Draw(pixel, perfect, Color.ForestGreen);
            sb.Draw(pixel, line, Color.MidnightBlue);

        }

        public void drawMessage(Vector2 pos, SpriteBatch sb)
        {
            icon = new Rectangle((int)pos.X + (int)(font.MeasureString(helpText[0]).X)+3, (int)pos.Y, 50, 50);
            sb.DrawString(font, helpText[0], new Vector2(pos.X, icon.Center.Y), Color.Blue);
            sb.Draw(InputAction.B.icon(), icon, Color.White);
            sb.DrawString(font, helpText[1], new Vector2(icon.Right+3, icon.Center.Y), Color.Blue);
        }

        public override void update(GameTime gameTime, InputState state)
        {


            if (startDelay > 0)
                startDelay -= gameTime.ElapsedGameTime.Milliseconds;
            else
            {
                base.update(gameTime, state);
                if(InputAction.B.wasPressed(state))
                {
                    finished = true;
                    multiplier = getMultiplier();
                }
                if (line.Right > track.Right)
                {
                    if(speed > 0)
                        speed *= -1f;
                }
                position += new Vector2(speed, 0);
                setLine();
                if (line.Left < track.Left - 1)
                {
                    finished = true;
                    multiplier = getMultiplier();
                }
                    
            }


        }

        public float getMultiplier()
        {
            if (line.Intersects(perfect))
            {
                SoundUtils.Play(SoundUtils.Sound.Complete);
                return 0.25f;
            }
                
            if (line.Intersects(fair))
            {
                SoundUtils.Play(SoundUtils.Sound.Success);
                return 0.75f;
            }
            SoundUtils.Play(SoundUtils.Sound.Failure);
            return 1f;
        }


        public void setLine()
        {
            line = new Rectangle((int)position.X + trackOffsetX, (int)position.Y + 50, 2, lineHeight);
        }

        public void setTrack()
        {
            track = new Rectangle((int)position.X + trackOffsetX , (int)position.Y + trackOffsetY, trackWidth, trackHeight);

            // Determine a random position for the success area
            Random r = new Random();
            int trackCentre = r.Next(fairWidth / 2, trackWidth - fairWidth / 2);

            perfect = new Rectangle(track.X + trackCentre - perfectWidth / 2, track.Top, perfectWidth, trackHeight);
            fair = new Rectangle(track.X + trackCentre - (fairWidth / 2), track.Top, fairWidth, trackHeight);
        }

        public override void load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/MainFontSmall");
        }

        public override int totalHeight()
        {
            throw new NotImplementedException();
        }

        public override int totalWidth()
        {
            throw new NotImplementedException();
        }

    }
}
