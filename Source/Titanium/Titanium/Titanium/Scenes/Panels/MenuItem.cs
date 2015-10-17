﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Scenes.Panels
{
    class MenuItem: Panel
    {
        public static int OFFSET = 50;

        string text;

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        SpriteFont font;

        InputAction action;

        public bool selected;
        public bool confirmed;

        Texture2D icon;

        Color textColor = Color.Black;
        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public MenuItem(string text, InputAction action, MenuPanel menu): base(menu, new Vector2(0, 0))
        {
            this.text = text;
            this.action = action;
            
        }

        public override void load(ContentManager content)
        {
            icon = InputAction.GetIcon(content, action);
            font = content.Load<SpriteFont>("TestFont");
        }

        public override void update(GameTime gametime, InputState inputState)
        {
            PlayerIndex player;
            if(action.Evaluate(inputState, null, out player))
            {
                if (selected)
                    confirmed = true;
                selected = true;
            }
            base.update(gametime, inputState);
        }

        public override void draw(SpriteBatch sb)
        {
            sb.DrawString(font, text, Position, textColor);
            sb.Draw(icon, new Vector2(Position.X - OFFSET, Position.Y), null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            base.draw(sb);
        }
        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight()
        {
            return font.LineSpacing;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth()
        {
            return (int)font.MeasureString(Text).X;
        }

    }
}
