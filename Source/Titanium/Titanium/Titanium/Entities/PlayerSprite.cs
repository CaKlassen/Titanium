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
using Microsoft.Xna.Framework.Content;

namespace Titanium.Entities
{
    

    public class PlayerSprite: Sprite
    {
        public Skill quick;
        public Skill skill1;
        public Skill skill2;

        Skill selectedSkill;
        public delegate void PlayerAction(Sprite s, float multiplier);

        public PlayerSprite(Skill s1, Skill s2): base()
        {
            skill1 = s1;
            skill2 = s2;

            quick = new Skill("Quick Attack", new Mash());

            skill1.assign(this, normalAttack);
            skill2.assign(this, strongAttack);
            quick.assign(this, quickAttack);
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

        public override void Load(ContentManager content)
        {
            quick.load(content);
            skill1.load(content);
            skill2.load(content);
            base.Load(content);
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
                sb.Draw(currentSpriteFile, destRect, sourceRect, Color.White);
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

        public string getName()
        {
            return rawStats.name;
        }


        public void Quick()
        {
            selectedSkill = quick;
        }

        public void Skill1()
        {
            selectedSkill = skill1;
        }
        public void Skill2()
        {
            selectedSkill = skill2;
        }

        public BaseGambit execute(Sprite target, GameTime gameTime)
        {
            return selectedSkill.execute(target, gameTime);
        }

        public void Resolve(float multiplier)
        {
            selectedSkill.resolve(multiplier);
        }
    }
}
