using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Gambits;
using Titanium.Battle;
using Titanium.Scenes.Panels;

namespace Titanium.Entities
{
    

    class PlayerSprite: Sprite
    {
        static InputAction quick;
        static InputAction normal;
        static InputAction strong;

        PlayerIndex player;
        Sprite target;
        public UnitState state;

        static PlayerSprite()
        {
            quick = new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { Keys.X },
                true
            );

            normal = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.A },
                true
            );

            strong = new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.B },
                true
            );
            
        }

        public PlayerSprite(Encounter encounter):base()
        {
            state = UnitState.idle;
        }

        public void quickAttack(Sprite s, float multiplier)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 0.5);
            s.takeDamage(damageDone);
        }

        public void normalAttack(Sprite s, float multiplier)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * multiplier);
            s.takeDamage(damageDone);
        }

        public  void strongAttack(Sprite s, float multiplier)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * multiplier);
            s.takeDamage(damageDone);
        }

        public override void Draw(SpriteBatch sb)
        {
            bool active = state == UnitState.selected || state == UnitState.gambit || state == UnitState.targeting;

            if(active)
                sb.Draw(spriteFile, destRect, sourceRect, Color.White);
            else
                sb.Draw(spriteFile, destRect, sourceRect, Color.Black);

            combatInfo.draw(sb);
        }

        public List<MenuItem> getMenuItems()
        {
            return new List<MenuItem>()
            {
                new MenuItem("Normal Attack", normal),
                new MenuItem("Quick Attack", quick),
                new MenuItem("Strong Attack", strong),
            };
        }

        public MenuPanel getMenuPanel()
        {
            return new MenuPanel(rawStats.name, getMenuItems());
        }

        public SpriteAction getAction(InputAction action, out BaseGambit gambit)
        {
            
            if (action == normal)
            {
                gambit = new Combo();
                return normalAttack;
            }
            if (action == strong)
            {
                gambit = new Mash();
                return strongAttack;
            }
            else
            {   
                gambit = null;
                return quickAttack;           
            }
        }

        public enum UnitState
        {
            idle,
            selected,
            gambit,
            targeting,
            resting
        }
    }
}
