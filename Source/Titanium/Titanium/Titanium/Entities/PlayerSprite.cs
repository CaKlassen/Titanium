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


        /// <summary>
        /// A quick attack that does not require a gambit
        /// </summary>
        /// <param name="s">The sprite to be attacked</param>
        /// <param name="multiplier">The multiplier of the attack</param>
        public void quickAttack(Sprite s, float multiplier)
        {
            targetRect = s.originalRect;
            changeState(State.Running);
            hitTarget(s, multiplier);
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
            hitTarget(s, multiplier);
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
            hitTarget(s, multiplier);
        }

        /// <summary>
        /// Draw the sprite depending on its state
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used to draw</param>
        public override void Draw(SpriteBatch sb)
        {
            if (checkDeath())
            {
                changeState(State.Hurt);
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.Black);
            }
            else
            {
                bool active = state == UnitState.selected || state == UnitState.gambit || state == UnitState.targeting;

                if (active)
                    sb.Draw(currentSpriteFile, destRect, sourceRect, Color.White);
                else
                    sb.Draw(currentSpriteFile, destRect, sourceRect, Color.Gray);

                combatInfo.draw(sb);
            }
        }

        /// <summary>
        /// Update this sprite if it has not already acted this turn
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            base.Update(gameTime, inputState);
        }

        
    }
}
