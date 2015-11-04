using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Scenes.Panels
{
    public class MenuItem: Panel
    {
        // The spacing between elements
        public static int OFFSET = 50;
        string text;

        public SpriteFont Font
        {
            get { return font; }
            set { font = value; }
        }
        SpriteFont font;

        public InputAction action;

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

        public MenuItem(string text, InputAction action): base()
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
            sb.Draw(icon, Position, null, Color.White, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            sb.DrawString(font, text, Position + new Vector2(50,0), textColor);
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
            return (int)font.MeasureString(Text).X + icon.Width;
        }

        /// <summary>
        /// Returns true if the user input the action representing this MenuItem
        /// </summary>
        /// <param name="inputState"></param>
        /// <returns></returns>
        public bool wasChosen(InputState inputState)
        {
            PlayerIndex player;
            return action.Evaluate(inputState, null, out player);
        }
    }
}
