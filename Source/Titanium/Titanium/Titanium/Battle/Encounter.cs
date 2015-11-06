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
        public EncounterState state;
        ContentManager content;

        bool resolved;

        PartyPanel party;
        EnemyPanel enemies;
        BattleMenuPanel battleMenu;

        Skill selectedSkill;
        BaseGambit currentGambit;

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
            battleMenu.center();
            resolved = true;
        }

        /// <summary>
        /// Draw this encounter
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used</param>
        public void draw(SpriteBatch sb)
        {
            switch(state)
            {
                case EncounterState.Gambit:
                    currentGambit.draw(sb);
                    break;
                case EncounterState.HeroSelect:
                    drawHeroIcons(sb);
                    break;
                case EncounterState.EnemySelect:
                    drawEnemyIcons(sb);
                    break;
                case EncounterState.ActionSelect:
                    battleMenu.draw(sb);
                    break;
                case EncounterState.Animating:
                case EncounterState.EnemyTurn:
                    break;
            }
            enemies.draw(sb);
            party.draw(sb);
        }

        public void drawHeroIcons(SpriteBatch sb)
        {
            Texture2D[] icons = { InputAction.Y.icon(), InputAction.X.icon(), InputAction.A.icon() };
            for(int i = 0; i<icons.Length; ++i)
            {
                if (party[i].checkDeath() || party[i].currentState == Sprite.State.Resting)
                    continue;
                sb.Draw(icons[i], party[i].getPosition(), Color.White);
            }
        }

        public void drawEnemyIcons(SpriteBatch sb) { }

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
                    if (InputAction.Y.wasPressed(inputState))
                        select(0);
                    else if (InputAction.X.wasPressed(inputState))
                        select(1);
                    else if (InputAction.A.wasPressed(inputState))
                        select(2);
                    break;
                case EncounterState.ActionSelect:
                    if (InputAction.Y.wasPressed(inputState))
                        act(0);
                    else if (InputAction.X.wasPressed(inputState))
                        act(1);
                    else if (InputAction.A.wasPressed(inputState))
                        act(2);
                    break;
                case EncounterState.EnemySelect:
                    if (InputAction.LB.wasPressed(inputState))
                        attack(0, gameTime);
                    else if (InputAction.LT.wasPressed(inputState))
                        attack(1, gameTime);
                    else if (InputAction.RB.wasPressed(inputState))
                        attack(2, gameTime);
                    else if (InputAction.RT.wasPressed(inputState))
                        attack(3, gameTime);
                    break;
                case EncounterState.Gambit:
                    currentGambit.update(gameTime, inputState);
                    float m;
                    if (currentGambit.isComplete(out m))
                    {
                        PartyUtils.getParty().ElementAt(battleMenu.selected).Resolve(m);
                        state = EncounterState.Animating;
                    }
                    break;
                case EncounterState.Animating:
                    if (party.idle())
                    {
                        if (party.hasActed())
                            state = EncounterState.EnemyTurn;
                        else
                            state = EncounterState.HeroSelect;
                    }
                    break;
                case EncounterState.EnemyTurn:
                    if (!enemies.acting(gameTime))
                        state = EncounterState.HeroSelect;
                    break;
                default:
                    if (InputAction.B.wasPressed(inputState))
                        state = EncounterState.HeroSelect;
                    break;
            }
            

            enemies.update(gameTime, inputState);
            party.update(gameTime, inputState);
            battleMenu.update(gameTime, inputState);
        }

        public void select(int n)
        {
            battleMenu.selected = n;
            state = EncounterState.ActionSelect;
        }


        public bool success()
        {
            if (!resolved)
            {
                resolved = true;
                return true;
            }
            return false;            
        }

        public bool failure()
        {
            if (!resolved)
            {
                resolved = true;
                return true;
            }

            return false;
        }

        public void act(int n)
        {
            switch(n)
            {
                case 0:
                    PartyUtils.getParty().ElementAt(battleMenu.selected).Quick();
                    state = EncounterState.EnemySelect;
                    break;
                case 1:
                    PartyUtils.getParty().ElementAt(battleMenu.selected).Skill1();
                    state = EncounterState.EnemySelect;
                    break;
                case 2:
                    PartyUtils.getParty().ElementAt(battleMenu.selected).Skill2();
                    state = EncounterState.EnemySelect;
                    break;
                default: break;
            }
        }

        public void attack(int n, GameTime gameTime)
        {
            Sprite target;
            if ((target = enemies.get(n)) != null)
            {
                currentGambit = PartyUtils.getParty().ElementAt(battleMenu.selected).execute(target, gameTime);
                state = EncounterState.Gambit;
            }
             
        }
    }
}
