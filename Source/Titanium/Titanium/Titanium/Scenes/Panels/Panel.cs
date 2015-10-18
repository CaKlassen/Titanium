using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Scenes.Panels
{
    /// <summary>
    /// Defines a 2d panel that can be placed on the game screen
    /// </summary>
    class Panel
    {
        // The panels that are nested in this one
        protected List<Panel> subPanels;
        
        /// <summary>
        /// The position of the panel relative to its origin
        /// </summary>
        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        Vector2 offset;

        /// <summary>
        /// The origin of this panel, top level panels will always
        /// have an origin of (0,0)
        /// </summary>
        public Vector2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        Vector2 origin;

        /// <summary>
        /// The actual position of the panel on the game screen
        /// </summary>
        public Vector2 Position
        {
            get { return Vector2.Add(origin, offset);  }
        }


        public Panel()
        {
            this.Origin = Vector2.Zero;
            this.Offset = Vector2.Zero;
            subPanels = new List<Panel>();
        }

        /// <summary>
        /// Initializes a new panel instance
        /// </summary>
        /// <param name="scene">The scene this panel should be drawn on</param>
        /// <param name="pos">The position </param>
        public Panel(Vector2 pos)
        {
            this.origin = Vector2.Zero;
            this.offset = pos;
            subPanels = new List<Panel>();
        }

        /// <summary>
        /// Creates an instance of a panel and adds it as a subpanel
        /// </summary>
        /// <param name="panel">The panel to add this one to</param>
        /// <param name="pos">The position of this panel relative to the origin</param>
        public Panel(Panel panel, Vector2 pos)
        {
            this.offset = pos;
            panel.addSubPanel(this);
            subPanels = new List<Panel>();
        }

        /// <summary>
        /// Run logic on this panel and its sub panels
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        /// <param name="inputState">The state of the input devices</param>
        public virtual void update(GameTime gameTime, InputState inputState)
        {
            foreach(Panel panel in subPanels)
                panel.update(gameTime, inputState);
        }

        /// <summary>
        /// Draw this panel and its sub panels
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public virtual void draw(SpriteBatch sb)
        {
            foreach (Panel panel in subPanels)
                panel.draw(sb);
        }

        public virtual void load(ContentManager content)
        {
            foreach (Panel panel in subPanels)
                panel.load(content);
        }

        /// <summary>
        /// Add a sub panel to this one
        /// </summary>
        /// <param name="subPanel"></param>
        public void addSubPanel(Panel subPanel)
        {
            subPanel.Origin = this.Position;
            subPanels.Add(subPanel);
        }

        /// <summary>
        /// Clear all the sub panels
        /// </summary>
        public void clearPanels()
        {
            subPanels.Clear();
        }

    }
}
