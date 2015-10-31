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
    

    public class PlayerSprite: Sprite
    {
        // The Input actions associated with the actions the player can perform
        static InputAction quick;
        static InputAction normal;
        static InputAction strong;

        /// <summary>
        /// The possible states that this unit can be in
        /// </summary>
        public enum UnitState
        {
            idle,
            selected,
            gambit,
            targeting,
            resting
        }
        public UnitState state;

        // Initialize the actions
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

        /// <summary>
        /// Creates an empty PlayerSprite - must load the information in before it is usable.
        /// </summary>
        public PlayerSprite():base()
        {
            state = UnitState.idle;
        }

        /// <summary>
        /// A quick attack that does not require a gambit
        /// </summary>
        /// <param name="s">The sprite to be attacked</param>
        /// <param name="multiplier">The multiplier of the attack</param>
        public void quickAttack(Sprite s, float multiplier)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 0.5);
            s.takeDamage(damageDone);
            state = UnitState.resting;
        }

        /// <summary>
        /// A normal attack that requires a Combo gambit
        /// </summary>
        /// <param name="s">The sprite to be attacked</param>
        /// <param name="multiplier">The multiplier of the attack</param>
        public void normalAttack(Sprite s, float multiplier)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * multiplier);
            s.takeDamage(damageDone);
            state = UnitState.resting;
        }

        /// <summary>
        /// A strong attack that requires the Mash gambit
        /// </summary>
        /// <param name="s">The sprite to be attacked</param>
        /// <param name="multiplier">The multiplier of the attack</param>
        public void strongAttack(Sprite s, float multiplier)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * multiplier);
            s.takeDamage(damageDone);
            state = UnitState.resting;
        }

        /// <summary>
        /// Draw the sprite depending on its state
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used to draw</param>
        public override void Draw(SpriteBatch sb)
        {
            bool active = state == UnitState.selected || state == UnitState.gambit || state == UnitState.targeting;

            if(active)
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.White);
            else
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.Gray);

            combatInfo.draw(sb);
        }

        /// <summary>
        /// Update this sprite if it has not already acted this turn
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            //if (state != UnitState.resting)
                base.Update(gameTime, inputState);
        }

        /// <summary>
        /// Get the possible player actions as a list of MenuItems to be used in the battle menu
        /// </summary>
        /// <returns></returns>
        public List<MenuItem> getMenuItems()
        {
            return new List<MenuItem>()
            {
                new MenuItem("Normal Attack", normal),
                new MenuItem("Quick Attack", quick),
                new MenuItem("Strong Attack", strong),
            };
        }

        /// <summary>
        /// Return a menu panel based on this sprite
        /// </summary>
        /// <returns>A MenuPanel whose title is this sprite's name</returns>
        public MenuPanel getMenuPanel()
        {
            return new MenuPanel(rawStats.name, getMenuItems());
        }

        /// <summary>
        /// Returns the appropriate action, and gambit if applicable, of the given InputAction
        /// </summary>
        /// <param name="action">The InputAction to resolve</param>
        /// <param name="gambit">The gambit that needs to be performed, null if none</param>
        /// <returns>The SpriteAction delegate to be called when this action is resolved.</returns>
        public SpriteAction getAction(InputAction action, out BaseGambit gambit)
        {
            
            if (action == normal)
            {
                gambit = new Combo();
                return normalAttack;
            }
            if (action == strong)
            {
                gambit = new Rotation();
                return strongAttack;
            }
            else
            {   
                gambit = null;
                return quickAttack;           
            }
        }

        

        
    }
}
