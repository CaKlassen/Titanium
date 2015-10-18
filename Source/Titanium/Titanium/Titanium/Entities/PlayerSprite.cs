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
        List<InputAction> actions;
        InputAction currentAction;

        BaseGambit currentGambit;

        delegate void Action(Sprite target);
        Action action;

        float multiplier;

        Encounter currentEncounter;

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
            actions = new List<InputAction>() { quick, normal, strong };

            currentEncounter = encounter;

            state = UnitState.idle;
        }

        public override void quickAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 0.8);
            s.takeDamage(damageDone);
        }

        public override void normalAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            s.takeDamage(damageDone);
        }

        public override void strongAttack(Sprite s)
        {
            int damageDone = 0;
            damageDone += this.rawStats.baseAttack + (int)Math.Round(this.rawStats.strength * 1.5);
            damageDone = (int)Math.Round(damageDone * 1.2);
            s.takeDamage(damageDone);
        }

        public override void Update(GameTime gameTime, InputState inputState)
        {
            switch(state)
            {
                case UnitState.idle:
                    if (normal.Evaluate(inputState, null, out player))
                        action = normalAttack;
                    else if (quick.Evaluate(inputState, null, out player))
                        action = quickAttack;
                    else if (strong.Evaluate(inputState, null, out player))
                        action = strongAttack;
                    state = UnitState.targeting;
                    break;
                case UnitState.gambit:
                    currentGambit.update(gameTime, inputState);
                    if (currentGambit.isComplete(out multiplier))
                    {
                        action(target);
                        state = UnitState.idle;
                    }
                    break;
                case UnitState.targeting:
                    if ((target = currentEncounter.targetSelected(inputState)) != null)
                        executeAction(gameTime);
                    break;
                default:
                    break;
            }
            
            base.Update(gameTime, inputState);
        }

        private void executeAction(GameTime gameTime)
        {
            if (action == normalAttack)
            {
                currentGambit = new Combo(gameTime);
                state = UnitState.gambit;
            }
            else if (action == strongAttack)
            {
                currentGambit = new Mash(gameTime);
                state = UnitState.gambit;
            }
            else if(action == quickAttack)
            {
                state = UnitState.idle;
                action(target);
            }
                
        }

        public override void Draw(SpriteBatch sb)
        {
            bool active = state == UnitState.selected || state == UnitState.gambit || state == UnitState.targeting;
            if (state == UnitState.gambit)
                currentGambit.draw(sb);

            sb.Draw(spriteFile, destRect, sourceRect, Color.White);
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
            return new MenuPanel(getMenuItems(), rawStats.name);
        }
        public enum UnitState
        {
            idle,
            selected,
            gambit,
            targeting
        }
    }
}
