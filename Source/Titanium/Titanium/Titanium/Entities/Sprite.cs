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
        protected Rectangle sourceRect, targetRect;
        public Rectangle destRect, originalRect;
        private double elapsed, delay;
        protected int frames, posX, posY, frameCount, hurtFrameCount, runFrameCount;
        protected UnitStats rawStats;
        protected CombatInfo combatInfo;

        public delegate void SpriteAction(Sprite target, float multiplier);

        //For testing purpose only
        protected Texture2D currentSpriteFile, idleFile, hurtFile, runFile;
        String filePath = "";

        public enum State { Idle, Running, FinishedRunning, Hurt, FinishedHurting }
        protected State currentState;

        public Sprite()
        {
            elapsed = 0;
            delay = 200;
            frames = 0;
            hurtFrameCount = 0;
            runFrameCount = 0;
            posX = 150;
            posY = 150;
            currentState = State.Idle;
        }


        public void Load(ContentManager content)
        {
            idleFile = content.Load<Texture2D>("Sprites/" + filePath + "_idle");
            runFile = content.Load<Texture2D>("Sprites/" + filePath + "_run");
            hurtFile = content.Load<Texture2D>("Sprites/" + filePath + "_hurt");
            currentSpriteFile = idleFile;
            destRect = new Rectangle(posX, posY, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
            originalRect = destRect;
            combatInfo = new CombatInfo();
            combatInfo.init(content, destRect);
            combatInfo.update(rawStats);
            frames = 4;
        }

        public void setParam(UnitStats u, int x, int y)
        {
            this.rawStats = u;
            this.posX = x;
            this.posY = y;
            this.filePath += rawStats.model;
            this.frameCount = rawStats.modelFrameCount;
            this.rawStats.normalize();
        }

        public override void Draw(SpriteBatch sb)
        {
            if (checkDeath())
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.Black);
            else
            {
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.White);
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
                if (currentState == State.Running)
                {
                    updateRun();
                }
                sourceRect = new Rectangle(currentSpriteFile.Width / frameCount * frames, 0, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                if (currentState == State.Hurt)
                {
                    if (hurtFrameCount >= 20)
                    {
                        changeState(State.Idle);
                        hurtFrameCount = 0;
                    }
                    else
                    {
                        hurtFrameCount++;
                    }
                }
            }
        }

        public enum Direction { Up, Down, Left, Right, None }
        public Direction animationDirectionLR = Direction.None, animationDirectionUD = Direction.None;

        public void updateRun()
        {
            bool changed = false;

            if (this.destRect.X + this.destRect.Width < this.targetRect.X && animationDirectionLR != Direction.Left)
            {
                animationDirectionLR = Direction.Right;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X+=5, tempRect.Y, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }
            else if (this.destRect.X > this.targetRect.X + this.targetRect.Width && animationDirectionLR != Direction.Right)
            {
                animationDirectionLR = Direction.Left;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X-=5, tempRect.Y, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }

            if (this.destRect.Y < this.targetRect.Y && animationDirectionUD != Direction.Up)
            {
                animationDirectionUD = Direction.Down;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X, tempRect.Y+=5, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }
            else if (this.destRect.Y > this.targetRect.Y && animationDirectionUD != Direction.Down)
            {
                animationDirectionUD = Direction.Up;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X, tempRect.Y-=5, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }

            if (!changed)
            {
                changeState(State.Idle);
                destRect = originalRect;
            }
        }


        public int getHealth() { return rawStats.currentHP; }
        public int getMana() { return rawStats.currentMP; }
       


        /// <summary>
        /// currently for Arena potion pick up.
        /// </summary>
        /// <param name="healPercent">percentage of health to heal</param>
        public void heal(float healPercent)
        {
            int healAmount = (int)healPercent / this.rawStats.baseHP;
            this.rawStats.currentHP += healAmount;
        }

        /**
        *COMBAT STARTS HERE
        **/
        public void takeDamage(int damage)
        {
            changeState(State.Hurt);
            this.rawStats.currentHP -= damage;
            checkDeath();
            combatInfo.update(rawStats);
        }

        public void useMana(int mana)
        {
            this.rawStats.currentMP -= mana;
        }

        public void changeState(State s)
        {
            switch(s)
            {
                case State.Running:
                    this.currentSpriteFile = runFile;
                    this.frameCount = 4;
                    break;
                case State.Hurt:
                    this.currentSpriteFile = hurtFile;
                    this.frameCount = 1;
                    break;
                case State.Idle:
                    this.currentSpriteFile = idleFile;
                    this.frameCount = 1;
                    break;
            }
            currentState = s;
        }

        public virtual bool quickAttack(Sprite s)
        {
            targetRect = s.originalRect;
            changeState(State.Running);

            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 0.8);
            s.takeDamage(damageDone);
            return false;
        }

        public virtual void normalAttack(Sprite s)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            s.takeDamage(damageDone);
        }

        public virtual void strongAttack(Sprite s)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 1.2);
            s.takeDamage(damageDone);
        }

        public Boolean checkDeath()
        {
            if (this.rawStats.currentHP <= 0)
            {
                changeState(State.Hurt);
                return true;
            }
            return false;
        }

        public int getWidth()
        {
            return currentSpriteFile.Width/frameCount;
        }

        public int getHeight()
        {
            return currentSpriteFile.Height;
        }

        public void move(int x, int y)
        {
            this.posX = x;
            this.posY = y;
            destRect = new Rectangle(posX, posY, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
            combatInfo.move(destRect);
            originalRect = destRect;
        }

        public Vector2 getPosition()
        {
            return new Vector2(posX, posY);
        }

    }
}