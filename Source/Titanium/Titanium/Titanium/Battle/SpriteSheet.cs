using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Battle
{
    class SpriteSheet
    {
        static int FRAMES = 4;
        static int POSES = 3;
        static int FRAME_DELAY = 20;        
        static string BASE_PATH = "Sprites/";

        int delay = 0;
        int currentFrame, currentPose;
        string path;
        public int width, height;
        bool initialized;
        Texture2D spritesheet;
        Rectangle source;

        public SpriteSheet(string path)
        {
            this.path = BASE_PATH + path + "-spritesheet";
            initialized = false;
        }

        public void update()
        {
            if (delay > FRAME_DELAY)
                nextFrame();

            delay++;
        }

        public void nextFrame()
        {
            delay = 0;
            if (currentFrame >= FRAMES)
                currentFrame = 0;
            source = new Rectangle(width * currentFrame, height * currentPose, width, height);
            currentFrame++;
        }
        public void load(ContentManager content)
        {
            this.spritesheet = content.Load<Texture2D>(path);
            this.width = spritesheet.Width / FRAMES;
            this.height = spritesheet.Height / POSES;
            initialized = true;
            currentFrame = 0;
            currentPose = 0;
            nextFrame();
        }

        public void draw(SpriteBatch sb, Rectangle destination)
        {
            sb.Draw(spritesheet, destination, source, Color.White);
        }

        public void draw(SpriteBatch sb, Rectangle destination, Color color)
        {
            sb.Draw(spritesheet, destination, source, color);
        }

        public void setPose(int n) { currentPose = n; }
    }
}
