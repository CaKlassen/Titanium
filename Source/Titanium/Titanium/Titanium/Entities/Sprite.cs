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
        protected int frames, posX, posY, frameCount, hurtFrameCount, runFrameCount, idleFrameCount;
        protected UnitStats rawStats;
        protected CombatInfo combatInfo;
        
        public delegate void SpriteAction(Sprite target);

        //For testing purpose only
        protected Texture2D currentSpriteFile, idleFile, hurtFile, runFile, shadow;

        static int offsetX = 30;
        static int offsetY = -20;

        String filePath = "";

        public enum Direction { Up, Down, Left, Right, None }
        public Direction animationDirectionLR = Direction.None, animationDirectionUD = Direction.None;
        public enum State { Idle, Running, FinishedRunning, Attacking, Returning, FinishedReturning, Hurt, Resting, Dead }
        public State currentState;
        public Sprite enemySprite;
        public float attackMultiplier;

        public Sprite(List<SpriteAction> actions)
        {
            elapsed = 0;
            delay = 200;
            frames = 0;
            hurtFrameCount = 0;
            runFrameCount = 0;
            posX = 150;
            posY = 150;
            changeState(State.Idle);
            attackMultiplier = 1.0f;
            combatInfo = new CombatInfo();
        }


        public virtual void Load(ContentManager content)
        {
            idleFile = content.Load<Texture2D>("Sprites/" + filePath + "_idle");
            runFile = content.Load<Texture2D>("Sprites/" + filePath + "_run");
            hurtFile = content.Load<Texture2D>("Sprites/" + filePath + "_hurt");
            currentSpriteFile = idleFile;
            destRect = new Rectangle(posX, posY, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
            originalRect = destRect;
            combatInfo.init(content, destRect);

            shadow = content.Load<Texture2D>("Sprites/Battle-Shadow");
        }

        public void setParam(UnitStats u, int x, int y)
        {
            this.rawStats = u;
            this.posX = x;
            this.posY = y;
            this.filePath += rawStats.model;
            this.frameCount = rawStats.modelFrameCount;
            this.idleFrameCount = this.frameCount;
            this.rawStats.normalize();
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {
            if (!checkDeath())
            {
                sb.Draw(shadow, new Vector2(destRect.Left + offsetX, destRect.Bottom + offsetY), Color.White);
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.White);
                combatInfo.draw(sb);
            } else
            {
                changeState(State.Dead);
            }
        }


        public override void Update(GameTime gameTime, InputState inputState)
        {
            if (!checkDeath())
            {
                elapsed += gameTime.ElapsedGameTime.TotalMilliseconds;
                combatInfo.update(rawStats);
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
                if (currentState == State.Attacking)
                {
                    int damageDone = 0;
                    damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * attackMultiplier);
                    enemySprite.takeDamage(damageDone);
                    animationDirectionLR = Direction.None;
                    animationDirectionUD = Direction.None;
                    changeState(State.Returning);
                }
                if (currentState == State.Returning)
                {
                    updateReturn();
                }
                if (currentState == State.FinishedReturning)
                {
                    animationDirectionLR = Direction.None;
                    animationDirectionUD = Direction.None;
                    changeState(State.Idle);
                }
            }
        }

        public void updateReturn()
        {
            bool changed = false;

            if (this.destRect.X < this.originalRect.X && animationDirectionLR != Direction.Left)
            {
                animationDirectionLR = Direction.Right;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X += 10, tempRect.Y, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }
            else if (this.destRect.X > this.originalRect.X && animationDirectionLR != Direction.Right)
            {
                animationDirectionLR = Direction.Left;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X -= 10, tempRect.Y, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }

            if (this.destRect.Y < this.originalRect.Y && animationDirectionUD != Direction.Up)
            {
                animationDirectionUD = Direction.Down;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X, tempRect.Y += 10, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }
            else if (this.destRect.Y > this.originalRect.Y && animationDirectionUD != Direction.Down)
            {
                animationDirectionUD = Direction.Up;
                Rectangle tempRect = destRect;
                destRect = new Rectangle(tempRect.X, tempRect.Y -= 10, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
                changed = true;
            }
            if (!changed)
            {
                changeState(State.FinishedReturning);
            }
        }

        //The animation of the character moving across the screen to attack
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
                changeState(State.Attacking);
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

            //if the heal amount puts the players HP higher than full health
            if (this.rawStats.currentHP + healAmount > this.rawStats.baseHP)
                this.rawStats.currentHP = this.rawStats.baseHP;//set the health to full health
            else
                this.rawStats.currentHP += healAmount;//otherwise heal by the healAmount
        }

        public int getBaseHP()
        {
            return this.rawStats.baseHP;
        }
        /**
        *COMBAT STARTS HERE
        **/
        public void takeDamage(int damage)
        {
            changeState(State.Hurt);
            int newHealth = this.rawStats.currentHP;
            newHealth -= damage;
            if (newHealth < 0)
                newHealth = 0;
            this.rawStats.currentHP = newHealth;
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
                case State.Returning:
                case State.Idle:
                case State.Resting:
                default:
                    this.currentSpriteFile = idleFile;
                    this.frameCount = this.idleFrameCount;
                    break;
            }
            currentState = s;
        }

        public void hitTarget(Sprite s, float multiplier)
        {
            this.enemySprite = s;
            this.attackMultiplier = multiplier;
            targetRect = s.originalRect;
            changeState(State.Running);
        }

        public bool checkDeath()
        {
            if (this.rawStats.currentHP <= 0)
            {
                changeState(State.Dead);
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



        public Vector2 getPosition()
        {
            return new Vector2(posX, posY);
        }

        public override Vector3 getPOSITION()
        {
            throw new NotImplementedException();
        }

        public void move(Vector2 v)
        {
            posX = (int)v.X;
            posY = (int)v.Y;
            destRect = new Rectangle(posX, posY, currentSpriteFile.Width / frameCount, currentSpriteFile.Height);
            originalRect = destRect;
            combatInfo.move(originalRect);
        }
    }
}