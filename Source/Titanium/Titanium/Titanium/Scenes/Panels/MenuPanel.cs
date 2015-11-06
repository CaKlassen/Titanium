using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Titanium.Scenes.Panels
{
    /// <summary>
    /// Class represents a single menu to be drawn on the game screen
    /// </summary>
    public class MenuPanel: Panel
    {
        string title;
        SpriteFont font;

        Color textColor = Color.DarkBlue;

        // Some padding so things aren't all squished
        private int SPACING = 5;
        public MenuPanel(string menuTitle, List<MenuItem> items) : base()
        {
            subPanels = items.Cast<Panel>().ToList();
            title = menuTitle;
        }

        public MenuPanel(string menuTitle) : base()
        {
            title = menuTitle;
        }

        /// <summary>
        /// Updates the location of each of the items in the menu.
        /// </summary>
        public void updateMenuItemLocations()
        {
            // The items should be below the title, if there is one
            float currentHeight = string.IsNullOrEmpty(title) ? 0 : font.LineSpacing;

            // Update the location of each menu item
            foreach (MenuItem item in subPanels)
            {
                item.Origin = Position;
                item.Offset = new Vector2(0, currentHeight + SPACING);
                currentHeight += item.GetHeight();
            }

        }

        public override void load(ContentManager content, Viewport v)
        {
            base.load(content, v);
            font = content.Load<SpriteFont>("TestFont");    
            updateMenuItemLocations();
        }

        /// <summary>
        /// The height of the menu.
        /// </summary>
        /// <returns>The total height of this menu</returns>
        public float totalHeight()
        {
            float h = string.IsNullOrEmpty(title) ? Position.Y : font.LineSpacing + Position.Y;
            foreach (MenuItem item in subPanels)
                h += item.GetHeight() + SPACING;
            return h;
        }

        /// <summary>
        /// The width of the menu.
        /// </summary>
        /// <returns>The total width of the menu.</returns>
        public float totalWidth()
        {
            float w = 0f;
            float tmp;
            foreach (MenuItem item in subPanels)
            {
                tmp = item.GetWidth();
                if (tmp > w)
                    w = tmp;
            }
            return w;
        }
        
        public override void draw(SpriteBatch sb)
        {
            sb.DrawString(font, title, Position, textColor);
            base.draw(sb);
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            base.update(gameTime, inputState);
        }

        /// <summary>
        /// Center this menu in the screen.
        /// </summary>
        public void center()
        {
            int width = v.Width;
            int height = v.Height;

            float x = width / 2 - totalWidth() / 2;
            float y = height / 2 - totalHeight() / 2;

            this.Origin = new Vector2(x, y);
            updateMenuItemLocations();
        }

        public void addMenuItem(MenuItem item)
        {
            addSubPanel(item);
        }
        
    }
}
