using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Titanium.Utilities;
using Titanium.Scenes;
using Microsoft.Xna.Framework.Input;

namespace Titanium.Entities
{
    public class Scoring : Entity
    {

        private InputAction cont = InputAction.A;
        Texture2D board;
        Texture2D Abutton;

        private static Vector2 DOWN_POS = new Vector2(0, 0);
        private static Vector2 UP_POS = new Vector2(0, -BaseGame.SCREEN_HEIGHT);
        public static int TEXT_X_POS = 450;
        private Vector2 boardPos;
        private Vector2 buttonPos;
        private SpriteFont font;
        private SpriteFont contFont;

        public int PotionBonus;
        public int HealthBonus;
        private int finalScore;
        private int GameScore;
        
        private bool done;
        private bool begin;
        private bool textDone;
        private bool down;

        const float TIMER = 1f;
        float timer = TIMER;
        private int text;

        bool show1 = false;
        bool show2 = false;
        bool show3 = false;
        bool show4 = false;
        bool show5 = false;

        public bool Begin
        {
            get { return begin; }
            set { begin = value; }
        }

        public Scoring()
        {
            boardPos = UP_POS;
            buttonPos = new Vector2(1140, 590);
            textDone = false;
            begin = false;        

            done = false;
            down = true;
            text = 0;
            GameScore = ArenaController.instance.getScore();
        }

        public void load(ContentManager Content)
        {
            board = Content.Load<Texture2D>("Sprites/Arena-Sign");

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                Abutton = Content.Load<Texture2D>("ButtonIcons/HUD-Face-A");
            }
            else
            {
                Abutton = Content.Load<Texture2D>("ButtonIcons/HUD-Key-Z");
            }

            DOWN_POS = new Vector2(0, BaseGame.SCREEN_HEIGHT - board.Height);

            font = Content.Load<SpriteFont>("Fonts/ScoreBoardFont");
            contFont = Content.Load<SpriteFont>("Fonts/MainFont");
        }

        public override void Draw(SpriteBatch sb, Effect effect)
        {


        }

        public void DrawScore(SpriteBatch sb)
        {
            sb.Begin();

            sb.Draw(board, boardPos, Color.White);

            if(boardPos == DOWN_POS && !textDone)
            {
                doText(sb);
            }

            if(show1)
                sb.DrawString(font, "Score for " + ArenaController.instance.getLevelType() + ", ACT - " + ArenaController.instance.getLevel(), new Vector2(TEXT_X_POS-70, 160), Color.Black);
            if(show2)
                sb.DrawString(font, "Potion Bonus: " + PotionBonus, new Vector2(TEXT_X_POS, 270), Color.Black);

            if(show3)
                sb.DrawString(font, "Health Bonus: " + HealthBonus, new Vector2(TEXT_X_POS, 360), Color.Black);

            if(show4)
                sb.DrawString(font, "Final Score: " + finalScore, new Vector2(TEXT_X_POS, 450), Color.Black);

            if (show5)
            {
                sb.DrawString(contFont, "Total Game Score: " + GameScore, new Vector2(90, buttonPos.Y + 20), Color.Black);
                sb.DrawString(contFont, "CONTINUE", new Vector2(buttonPos.X - 110, buttonPos.Y + 20), Color.Black);
                sb.Draw(Abutton, buttonPos, Color.White);
            }

            if (textDone)
                done = true;
                       

            sb.End();

        }

        private void doText(SpriteBatch sb)
        {

            if (timer <= 0)
            {
                switch (text)
                {
                    case 0:
                        GameScore += HighScoreUtils.CalculateHighScore(1000, 250);
                        SoundUtils.Play(SoundUtils.Sound.Complete);
                        show1 = true;
                        text++;
                        timer = TIMER;
                        break;
                    case 1:
                        SoundUtils.Play(SoundUtils.Sound.Complete);
                        show2 = true;
                        text++;
                        timer = TIMER;
                        break;
                    case 2:
                        SoundUtils.Play(SoundUtils.Sound.Complete);
                        show3 = true;
                        text++;
                        timer = TIMER;
                        break;
                    case 3:
                        finalScore = PotionBonus + HealthBonus;
                        SoundUtils.Play(SoundUtils.Sound.Complete);
                        show4 = true;
                        text++;
                        timer = TIMER;
                        break;
                    case 4:
                        show5 = true;
                        textDone = true;
                        break;
                }
            }
        }


        public override void Update(GameTime gameTime, InputState inputState)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsedTime;

            PlayerIndex player;
            if (cont.Evaluate(inputState, null, out player) && done)
            {
                begin = false;
                done = false;
                timer = TIMER;
                text = 0;
                textDone = false;
                ArenaController.instance.moveToNextArena();
            }

            if (down)
                boardPos.Y += MathUtils.smoothChange(boardPos.Y, DOWN_POS.Y, 10);

            if (boardPos.Y >= -1)
            {
                down = false;

                boardPos = DOWN_POS;
            }

        }

        public override Vector3 getPOSITION()
        {
            throw new NotImplementedException();
        }
    }
}
