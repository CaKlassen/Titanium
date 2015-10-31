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
        Texture2D barFrame;
        Rectangle destRect, frameRect;
        Color hpColor;
        String name = "";
        int curHP, maxHP;
        SpriteFont myFont;

        public CombatInfo()
        {
        }

        public void init(ContentManager content, Rectangle r)
        {
            barFrame = content.Load<Texture2D>("Sprites/HealthBar");
            myFont = content.Load<SpriteFont>("combat_font");
            Rectangle tempRect = r;
            frameRect = new Rectangle(tempRect.X + tempRect.Width, tempRect.Y, barFrame.Width / 2, 20);
            destRect = frameRect;
        }
        
        public void update(UnitStats u)
        {
            //hp calc
            this.name = u.name;


            float hpPercent = (float)u.currentHP / u.baseHP;
            float newLength = destRect.Width * hpPercent;

            destRect = new Rectangle(destRect.X, destRect.Y, (int)newLength, 20);

            if (hpPercent > 0.75)
            {
                hpColor = Color.LimeGreen;
            }
            else if (hpPercent > 0.25 && hpPercent < 0.75)
            {
                hpColor = Color.Yellow;
            }
            else
            {
                hpColor = Color.Red;
            }


        }

        public void draw(SpriteBatch sb)
        {
            sb.Draw(barFrame, frameRect, new Rectangle(0, 0, barFrame.Width, barFrame.Height / 2), Color.White);
            sb.Draw(barFrame, destRect, new Rectangle(0, barFrame.Height / 2, barFrame.Width,  barFrame.Height / 2), hpColor);
            sb.DrawString(myFont, name, new Vector2(destRect.X, destRect.Y - 20), Color.Black);
        }

        public void move(Rectangle tempRect)
        {
            if(tempRect.X < GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2)
            {
                frameRect = new Rectangle(tempRect.X - barFrame.Width/2, tempRect.Y + 50, barFrame.Width / 2, 20);
            } else
            {
                frameRect = new Rectangle(tempRect.X + tempRect.Width, tempRect.Y + 50, barFrame.Width / 2, 20);
            }
            destRect = frameRect;
        }





    }
}
