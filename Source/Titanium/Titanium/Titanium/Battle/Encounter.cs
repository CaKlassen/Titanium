using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;
using Titanium.Gambits;
using Titanium.Scenes.Panels;
using Titanium.Utilities;

namespace Titanium.Battle
{
    /// <summary>
    /// This object represents a single "battle" between the player and a set of enemies
    /// </summary>
    public class Encounter
    {
        public enum EncounterState
        {
            HeroSelect,
            ActionSelect,
            EnemySelect,
            Gambit,
            Animating,
            EnemyTurn,
        }

        static InputAction[] HeroSelectActions =            
        {
            InputAction.Y,
            InputAction.X,
            InputAction.A
        };
        static InputAction[] ActionSelectActions =
        {
            InputAction.Y,
            InputAction.X,
            InputAction.A
        };
        static InputAction[] EnemySelectActions =
        {
            InputAction.LB,
            InputAction.LT,
            InputAction.RB,
            InputAction.RT
        };

        public EncounterState state;
        ContentManager content;

        PartyPanel party;
        EnemyPanel enemies;
        BattleMenuPanel battleMenu;

        PlayerSprite selectedHero;
        Skill selectedSkill;
        Sprite targetedEnemy;

        GambitResult result;

        /// <summary>
        /// Initializes a blank Encounter for debug / bug fixing purposes
        /// </summary>
        public Encounter(List<PartyUtils.Enemy> front, List<PartyUtils.Enemy> back)
        {
            enemies = new EnemyPanel(this, PartyUtils.makeEnemies(front), PartyUtils.makeEnemies(back));
            party = new PartyPanel(this);
            battleMenu = new BattleMenuPanel(this);
        }

        /// <summary>
        /// Load the two sprite panels and save a reference to the content manager
        /// </summary>
        /// <param name="content"></param>
        public void load(ContentManager content, Viewport v)
        {
            this.content = content;
            enemies.load(content, v);
            party.load(content, v);
            battleMenu.load(content, v);
        }

        /// <summary>
        /// Draw this encounter
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used</param>
        public void draw(SpriteBatch sb, Effect effect)
        {
            battleMenu.draw(sb, effect);
            switch (state)
            {
                case EncounterState.HeroSelect:
                    drawHeroIcons(sb);
                    break;
                case EncounterState.EnemySelect:
                    drawEnemyIcons(sb);
                    break;
                default:
                    break;
            }
            
            enemies.draw(sb, effect);
            party.draw(sb, effect);
            
        }

        public void drawHeroIcons(SpriteBatch sb)
        {
            for(int i = 0; i<HeroSelectActions.Length; ++i)
            {
                if (party[i].checkDeath() || party[i].currentState == Sprite.State.Resting)
                    continue;
                sb.Draw(HeroSelectActions[i].icon(), party[i].getPosition(), Color.White);
            }
        }

        public void drawEnemyIcons(SpriteBatch sb)
        {
            for (int i = 0; i < EnemySelectActions.Length; ++i)
            {
                if (enemies[i] == null)
                    continue;
                if (enemies[i].checkDeath())
                    continue;
                sb.Draw(EnemySelectActions[i].icon(), enemies[i].getPosition(), Color.White);
            }
        }

        /// <summary>
        /// This function will detect the current state of the battle and update the necessary components only
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs to be used for input handling</param>
        public void update(GameTime gameTime, InputState inputState)
        {
            switch(state)
            {
                case EncounterState.HeroSelect:
                    for(int i = 0; i< HeroSelectActions.Count(); ++i)
                        if (HeroSelectActions[i].wasPressed(inputState))
                        {
                            selectHero(party[i]);
                            break;
                        }
                    break;
                case EncounterState.ActionSelect:
                    for (int i = 0; i < ActionSelectActions.Count(); ++i)
                        if (ActionSelectActions[i].wasPressed(inputState))
                        {
                            selectSkill(selectedHero.skills[i]);
                            break;
                        }
                    break;
                case EncounterState.EnemySelect:
                    for (int i = 0; i < EnemySelectActions.Count(); ++i)
                        if (EnemySelectActions[i].wasPressed(inputState))
                        {
                            selectEnemy(enemies[i], gameTime);
                            break;
                        }
                    break;
                case EncounterState.Gambit:
                    if (battleMenu.gambitComplete(out result))
                    {
                        selectedSkill.resolve(selectedHero, targetedEnemy, enemies, result);
                        state = EncounterState.Animating;
                    }
                    break;
                case EncounterState.Animating:
                    if (party.idle())
                    {
                        if (party.hasActed())
                        {
                            state = EncounterState.EnemyTurn;
                            enemies.activate(gameTime);
                        }
                        else
                            state = EncounterState.HeroSelect;
                    }
                    break;
                case EncounterState.EnemyTurn:
                    if (!enemies.acting())
                    {
                        state = EncounterState.HeroSelect;
                        party.activate();
                    }
                    break;
                default:
                    break;
            }
            

            enemies.update(gameTime, inputState);
            party.update(gameTime, inputState);
            battleMenu.update(gameTime, inputState);
        }

        public void selectHero(PlayerSprite hero)
        {             
            if (hero.currentState != Sprite.State.Resting && hero.currentState != Sprite.State.Dead)
            {
                selectedHero = hero;
                state = EncounterState.ActionSelect;
            }
        }

        public void selectSkill(Skill skill)
        {
            selectedSkill = skill;
            state = EncounterState.EnemySelect;
        }

        public void selectEnemy(Sprite target, GameTime gameTime)
        {
            if (target != null && target.currentState != Sprite.State.Dead)
            {
                targetedEnemy = target;
                battleMenu.start(selectedSkill.gambit, gameTime);
                state = EncounterState.Gambit;
            }

        }

        public bool success()
        {
            if(enemies.dead())
            {
                party.reset();
                return true;
            }
            return enemies.dead();        
        }

        public bool failure()
        {
            if(party.dead())
            {
                party.reset();
                return true;
            }

            return false;
        }

        

        
    }
}
