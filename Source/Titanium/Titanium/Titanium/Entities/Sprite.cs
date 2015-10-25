using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Titanium.Battle;

namespace Titanium.Entities
{
    public class Sprite : Entity
    {
        protected Rectangle sourceRect, destRect;
        private double elapsed, delay;
        private int frames, posX, posY, frameCount;
        protected UnitStats rawStats;
        protected CombatInfo combatInfo;

        public delegate void SpriteAction(Sprite target, float multiplier);

        //For testing purpose only
        protected Texture2D spriteFile;
        String filePath = "";

        public Sprite()
        {
            elapsed = 0;
            delay = 200;
            frames = 0;
            posX = 150;
            posY = 150;
        }


        public void Load(ContentManager content)
        {
            spriteFile = content.Load<Texture2D>("Sprites/" + filePath);
            destRect = new Rectangle(posX, posY, spriteFile.Width / frameCount, spriteFile.Height);
            combatInfo = new CombatInfo();
            combatInfo.init(content, destRect);
            combatInfo.update(rawStats);
        }

        public void setParam(UnitStats u, int x, int y)
        {
            this.rawStats = u;
            this.posX = x;
            this.posY = y;
            this.filePath += rawStats.model + "_idle";
            this.frameCount = rawStats.modelFrameCount;
            this.rawStats.normalize();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (checkDeath())
                sb.Draw(spriteFile, destRect, sourceRect, Color.Black);
            else
            {
                sb.Draw(spriteFile, destRect, sourceRect, Color.White);
                combatInfo.draw(sb);
            }

        }


        public override void Update(GameTime gameTime, InputState inputState)
        {
            if (!checkDeath())
            {
                elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (elapsed >= delay)
                {
                    if (frames >= (frameCount - 1))
                    {
                        frames = 0;
                    }
                    else
                    {
                        frames++;
                    }
                    elapsed = 0;
                }
                sourceRect = new Rectangle(spriteFile.Width / frameCount * frames, 0, spriteFile.Width / frameCount, spriteFile.Height);
            }
        }

        public int getHealth() { return rawStats.currentHP; }
        public int getMana() { return rawStats.currentMP; }
       




        /**
        *COMBAT STARTS HERE
        **/
        public void takeDamage(int damage)
        {
            Console.WriteLine(rawStats.name + " has " + rawStats.currentHP + " hp.");
            this.rawStats.currentHP -= damage;
            Console.WriteLine(rawStats.name + " has taken " + damage + " damage!");
            checkDeath();
            combatInfo.update(rawStats);
        }

        public void useMana(int mana)
        {
            this.rawStats.currentMP -= mana;
        }

        public virtual void quickAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 0.8);
            s.takeDamage(damageDone);
        }

        public virtual void normalAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            s.takeDamage(damageDone);
        }

        public virtual void strongAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 1.2);
            s.takeDamage(damageDone);
        }

        public Boolean checkDeath()
        {
            if (this.rawStats.currentHP <= 0)
                return true;
            return false;
        }

        public int width()
        {
            return spriteFile.Width/frameCount;
        }

        public int height()
        {
            return spriteFile.Height;
        }

        public void move(int x, int y)
        {
            this.posX = x;
            this.posY = y;
            destRect = new Rectangle(posX, posY, spriteFile.Width / frameCount, spriteFile.Height);
            combatInfo.move(destRect);
        }

        public Vector2 getPosition()
        {
            return new Vector2(posX, posY);
        }

    }
}