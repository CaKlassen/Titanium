using System;  
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Titanium.Entities;
using Microsoft.Xna.Framework;
using Titanium.Gambits;

namespace Titanium.Scenes.Panels
{
    /// <summary>
    /// Class contains a menu for each player controlled sprite.
    /// </summary>
    public class BattleMenuPanel: MenuPanel
    {


        public BattleMenuPanel(): base("Battle Menu")
        {
            
        }


        public override void load(ContentManager content, Viewport v)
        {
            base.load(content, v);
        }

        public override void draw(SpriteBatch sb)
        {
            base.draw(sb);
        }

        public override void update(GameTime gameTime, InputState inputState)
        {
            base.update(gameTime, inputState);
        }

    }
}
