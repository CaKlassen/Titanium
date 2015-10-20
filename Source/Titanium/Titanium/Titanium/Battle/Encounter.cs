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
    class Encounter
    {
        enum BattleState
        {
            idle,
            targeting,
            gambit,
            enemy
        }

        BattleState state;
        PlayerSpritePanel heroes;
        SpritePanel enemies;
        Sprite target;

        BaseGambit currentGambit;
        Sprite.SpriteAction pendingAction;

        float multiplier = 1f;

        static List<InputAction> targetActions;
        static InputAction heroNext;
        static InputAction heroPrev;
        static InputAction cancel;

        ContentManager content;

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

        public Encounter(List<PlayerSprite> heroes, List<Sprite> enemies)
        {
            this.heroes = new PlayerSpritePanel(heroes, SpritePanel.Side.east);
            this.enemies = new SpritePanel(enemies, SpritePanel.Side.west);
            state = BattleState.idle;
        }

        public Encounter()
        {
            /************************************************
            Sprite Creation Area; to be done via file parsing
            ************************************************/
            List<PlayerSprite> heroList = new List<PlayerSprite>()
            {
                new PlayerSprite(),
                new PlayerSprite()
            };
            loadStats(heroList.Cast<Sprite>().ToList(), "PlayerFile.txt");
            heroes = new PlayerSpritePanel(heroList, SpritePanel.Side.east);

            List<Sprite> enemyList = new List<Sprite>
            {
                new Sprite(),
                new Sprite()
            };
            loadStats(enemyList, "Stage_1_1.txt");
            enemies = new SpritePanel(enemyList, SpritePanel.Side.west);


        }

        public void loadStats(List<Sprite> l, String target)
        {
            String path = "../../../../TitaniumContent/Stats/";
            List<UnitStats> tempList = new List<UnitStats>();
            FileUtils myFileUtil = new FileUtils();
            tempList = myFileUtil.FileToSprite(path + target);
            for (int i = 0; i < l.Count; ++i)
                l[i].setParam(tempList[i], (int)Vector2.Zero.X, (int)Vector2.Zero.Y);
        }

        public void load(ContentManager content)
        {
            this.content = content;
            heroes.load(content);
            enemies.load(content);
        }

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

        public void update(GameTime gameTime, InputState inputState)
        {
            PlayerIndex player;
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
