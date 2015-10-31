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
        /// <summary>
        /// The possible states of the current battle
        /// </summary>
        enum BattleState
        {
            idle,
            targeting,
            gambit,
            enemy
        }
        BattleState state;

        // The player characters
        PlayerSpritePanel heroes;

        SpritePanel enemies;

        // The current target of the player
        Sprite target;

        // The gambit currently being played
        BaseGambit currentGambit;

        // The action to resolve after the current gambit is finished
        Sprite.SpriteAction pendingAction;

        // The multiplier strength of the attack
        float multiplier = 1f;

        // The possible actions to target an enemy
        static List<InputAction> targetActions;

        // Actions to select the next hero, previous hero and cancel target selection
        static InputAction heroNext;
        static InputAction heroPrev;
        static InputAction cancel;

        ContentManager content;

        /// <summary>
        /// Initialize the actions
        /// </summary>
        static Encounter()
        {
            targetActions = new List<InputAction>()
            {
                new InputAction(
                    new Buttons[] { Buttons.A },
                    new Keys[] { Keys.A },
                    true
                ),

                new InputAction(
                    new Buttons[] { Buttons.X },
                    new Keys[] { Keys.X },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.Y },
                    new Keys[] { Keys.Y },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.LeftShoulder },
                    new Keys[] { Keys.D1 },
                    true
                ),
                new InputAction(
                    new Buttons[] { Buttons.RightShoulder },
                    new Keys[] { Keys.D2 },
                    true
                )
            };

            heroNext = new InputAction(
                    new Buttons[] { Buttons.RightShoulder },
                    new Keys[] { Keys.D2 },
                    true
                );


            heroPrev = new InputAction(
                    new Buttons[] { Buttons.LeftShoulder },
                    new Keys[] { Keys.D1 },
                    true
                );

            cancel = new InputAction(
                    new Buttons[] { Buttons.B },
                    new Keys[] { Keys.B },
                    true
                );
        }

        /// <summary>
        /// This object represents a single "battle" between the player and a set of enemies
        /// </summary>
        /// <param name="heroes">The list of PlayerSprites that the player controls</param>
        /// <param name="enemies">The list of enemies the player is fighting</param>
        public Encounter(List<PlayerSprite> heroes, List<Sprite> enemies)
        {
            this.heroes = new PlayerSpritePanel(heroes, SpritePanel.Side.east);
            this.enemies = new SpritePanel(enemies, SpritePanel.Side.west);
            state = BattleState.idle;
        }

        /// <summary>
        /// Initializes a blank Encounter for debug / bug fixing purposes
        /// </summary>
        public Encounter()
        {
            /************************************************
            Sprite Creation Area; to be done via file parsing
            ************************************************/
            List<PlayerSprite> heroList = new List<PlayerSprite>()
            {
                new PlayerSprite(),
                new PlayerSprite(),
                new PlayerSprite()
            };
            loadStats(heroList.Cast<Sprite>().ToList(), "PlayerFile.txt");
            heroes = new PlayerSpritePanel(heroList, SpritePanel.Side.west);

            List<Sprite> enemyList = new List<Sprite>
            {
                new Sprite(),
                new Sprite()
            };
            loadStats(enemyList, "Stage_1_1.txt");
            enemies = new SpritePanel(enemyList, SpritePanel.Side.east);


        }


        public void loadStats(List<Sprite> l, String target)
        {
            String path = "Content/Stats/";
            List<UnitStats> tempList = new List<UnitStats>();
            FileUtils myFileUtil = new FileUtils();
            tempList = myFileUtil.FileToSprite(path + target);
            for (int i = 0; i < l.Count; ++i)
                l[i].setParam(tempList[i], (int)Vector2.Zero.X, (int)Vector2.Zero.Y);
        }

        /// <summary>
        /// Load the two sprite panels and save a reference to the content manager
        /// </summary>
        /// <param name="content"></param>
        public void load(ContentManager content)
        {
            this.content = content;
            heroes.load(content);
            enemies.load(content);
        }

        /// <summary>
        /// Draw this encounter
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used</param>
        public void draw(SpriteBatch sb)
        {
            switch (state)
            {
                case BattleState.targeting:
                    break;
                case BattleState.enemy:
                    break;
                case BattleState.gambit:
                    currentGambit.draw(sb);
                    break;
                case BattleState.idle:
                default:
                    break;
            }

            heroes.draw(sb);
            enemies.draw(sb);
        }

        /// <summary>
        /// This function will detect the current state of the battle and update the necessary components only
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs to be used for input handling</param>
        public void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;

            // If all the player characters have acted then it is the enemy's turn
            if (heroes.finished())
                state = BattleState.enemy;
            
            switch (state)
            {
                case BattleState.targeting:
                    if (heroPrev.Evaluate(inputState, null, out player))
                        heroes.selectPrevious();
                    else if (heroNext.Evaluate(inputState, null, out player))
                        heroes.selectNext();
                    else if (cancel.Evaluate(inputState, null, out player))
                    {
                        state = BattleState.idle;
                        enemies.target(false, targetActions);
                    }
                    else
                    {
                        enemies.target(true, targetActions);
                        target = targetSelected(inputState);
                        if (target != null)
                        {
                            if (currentGambit == null)
                            {
                                pendingAction(target, multiplier);
                                state = BattleState.idle;
                                heroes.selectNext();
                            }
                            else
                            {
                                currentGambit.load(content);
                                currentGambit.start(gameTime);
                                state = BattleState.gambit;
                            }
                            enemies.target(false, null);
                        }
                    }
                    break;
                case BattleState.enemy:
                    enemies.act(heroes.Sprites());
                    heroes.activate();
                    state = BattleState.idle;
                    break;
                case BattleState.gambit:
                    currentGambit.update(gameTime, inputState);
                    if (currentGambit.isComplete(out multiplier))
                    {
                        pendingAction(target, multiplier);
                        state = BattleState.idle;
                        heroes.selectNext();
                    }
                    break;
                case BattleState.idle:
                    if (heroPrev.Evaluate(inputState, null, out player))
                        heroes.selectPrevious();
                    else if (heroNext.Evaluate(inputState, null, out player))
                        heroes.selectNext();
                    else if (heroes.finished())
                        state = BattleState.enemy;
                    else
                    {
                        pendingAction = heroes.getAction(inputState, out currentGambit);
                        if (pendingAction != null)
                            state = BattleState.targeting;
                    }
                    break;
                default:
                    break;
            }

            heroes.update(gameTime, inputState);
            enemies.update(gameTime, inputState);
        }

        // Returns the enemy associated with the user's target input
        public Sprite targetSelected(InputState inputState)
        {
            PlayerIndex player;
            for (int i = 0; i < enemies.count(); ++i)
            {
                if (targetActions[i].Evaluate(inputState, null, out player))
                    return enemies.at(i);
            }
            return null;
        }

        /// <summary>
        /// Returns the result of this encounter
        /// </summary>
        /// <returns>true if all the enemies are dead, false if all the players are dead. null otherwise.</returns>
        public bool? outcome()
        {
            if (enemies.dead())
                return true;
            else if (heroes.dead())
                return false;
            else
                return null;
        }
    }
}
