using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Battle
{
    public class CombatInfo
    {
        Texture2D barFrame, barFrame2;
        Rectangle destRect, frameRect, destRectArena, frameRectArena;
        Color hpColor;
        String name = "";
        String health = "";
        int curHP, maxHP;
        float hpLength;
        SpriteFont numbersFontSmall, numbersFontLarge, hpFont;
        UnitStats unitStats;

        public object GraphicDevice { get; private set; }

        public CombatInfo()
        {
            unitStats = new UnitStats();
        }

        public void init(ContentManager content, Rectangle r)
        {
            barFrame = content.Load<Texture2D>("Sprites/HealthBar2");
            numbersFontSmall = content.Load<SpriteFont>("Fonts/MainFontSmall");
            hpFont = content.Load<SpriteFont>("Fonts/NumbersFont");
            Rectangle tempRect = r;
            frameRect = new Rectangle(tempRect.X, tempRect.Y-barFrame.Height/2, barFrame.Width, barFrame.Height/2);
            destRect = frameRect;
        }

        public void init(ContentManager content, Vector2 v)
        {
            barFrame2 = content.Load<Texture2D>("Sprites/HealthBar2");
            numbersFontSmall = content.Load<SpriteFont>("Fonts/MainFontSmall");
            numbersFontLarge = content.Load<SpriteFont>("Fonts/NumbersFontBig");
            hpFont = content.Load<SpriteFont>("Fonts/NumbersFont");
            frameRectArena = new Rectangle((int)v.X, (int)v.Y, barFrame2.Width, barFrame2.Height/2);
            destRectArena = frameRectArena;
        }
        
        public void update(UnitStats u)
        {
            //hp calc
            if (this.name.CompareTo("") == 0)
                this.name = u.name;

            unitStats = u;

            float hpPercent = (float)u.currentHP / u.baseHP;
            float newLength = frameRect.Width * hpPercent;

            destRect = new Rectangle(destRect.X, destRect.Y, (int)newLength, destRect.Height);

            health = u.currentHP + "/" + u.baseHP;

            if (hpPercent > 0.75)
            {
                hpColor = Color.LimeGreen;
            }
            else if (hpPercent > 0.25 && hpPercent <= 0.75)
            {
                hpColor = Color.Yellow;
            }
            else
            {
                hpColor = Color.Red;
            }
        }

        public void smallUpdate(Rectangle r)
        {
            Rectangle tempRect = r;
            frameRect = new Rectangle(tempRect.X, tempRect.Y-barFrame.Height/2, barFrame.Width, barFrame.Height/2);
            destRect = frameRect;
            update(unitStats);
        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(barFrame, new Rectangle(destRect.X, destRect.Y + 2, destRect.Width, destRect.Height - 3), new Rectangle(0, barFrame.Height / 2 + 1, barFrame.Width, barFrame.Height / 2), hpColor);
            sb.Draw(barFrame, frameRect, new Rectangle(0, 0, barFrame.Width, barFrame.Height / 2), Color.White);
            if (frameRect.X <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 3)
            {
                sb.DrawString(numbersFontSmall, name, new Vector2(frameRect.X, frameRect.Y - 20), Color.SteelBlue);
            }
            else
            {
                sb.DrawString(numbersFontSmall, name, new Vector2(frameRect.X, frameRect.Y - 20), Color.Tomato);
            }

            if (frameRect.X <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 3)
            {
                Vector2 spacing = hpFont.MeasureString(health);
                sb.DrawString(hpFont, health, new Vector2(frameRect.X+frameRect.Width-spacing.X-6, frameRect.Y+2), Color.White);
            }
            else
            {
                //enemy hp maybe
            }
        }

        public void drawArena(SpriteBatch sb)
        {
            sb.Draw(barFrame2, new Rectangle(destRectArena.X, destRectArena.Y+1, destRectArena.Width, destRectArena.Height-1), new Rectangle(0, (barFrame2.Height/2)+1, barFrame2.Width, barFrame2.Height/2), hpColor);
            sb.Draw(barFrame2, frameRectArena, new Rectangle(0, 0, barFrame2.Width, barFrame2.Height / 2), Color.White);
            Vector2 spacing = numbersFontSmall.MeasureString(name);
            sb.DrawString(numbersFontSmall, name, new Vector2(frameRectArena.X, frameRectArena.Y-spacing.Y), Color.White);
            spacing = hpFont.MeasureString(health);
            sb.DrawString(hpFont, health, new Vector2(frameRectArena.X + frameRectArena.Width - spacing.X - 6, frameRectArena.Y + 2), Color.White);
        }

        public void move(Rectangle tempRect)
        {
            frameRect = new Rectangle(tempRect.X, tempRect.Y, barFrame.Width, barFrame.Height/2);
            destRect = frameRect;
        }





    }
}
