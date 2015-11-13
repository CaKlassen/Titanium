using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Titanium.Gambits;
using Titanium.Battle;
using Titanium.Scenes.Panels;
using Microsoft.Xna.Framework.Content;
using Titanium.Utilities;

namespace Titanium.Entities
{
    

    public class PlayerSprite: Sprite
    {
        List<Skill> skills;

        Skill selectedSkill;

        public PlayerSprite(List<Skill> skills): base(new List<SpriteAction>())
        {
            this.skills = new List<Skill>();
            
            this.skills.Add(new Skill("Quick Attack", new Quick(), PartyUtils.testAction));
            this.skills = this.skills.Concat(skills).ToList();
        }

        public override void Load(ContentManager content)
        {
            foreach (Skill skill in skills)
                skill.load(content);

            base.Load(content);
        }

        /// <summary>
        /// Draw the sprite depending on its state
        /// </summary>
        /// <param name="sb">The SpriteBatch to be used to draw</param>
        public override void Draw(SpriteBatch sb, Effect effect)
        {
            base.Draw(sb, null);
        }

        /// <summary>
        /// Update this sprite if it has not already acted this turn
        /// </summary>
        /// <param name="gameTime">The current GameTime object</param>
        /// <param name="inputState">The state of the inputs</param>
        public override void Update(GameTime gameTime, InputState inputState)
        {
            base.Update(gameTime, inputState);
        }


        public BaseGambit execute(Sprite target, GameTime gameTime)
        {
            return selectedSkill.execute(target, gameTime);
        }

        public void resolve(GambitResult result)
        {
            selectedSkill.resolve(this, result);
        }

        public string name() { return rawStats.name; }

        public MenuPanel makeMenuPanel()
        {
            List<MenuItem> items = new List<MenuItem>();
            List<InputAction> actions = new List<InputAction>() { InputAction.Y, InputAction.A, InputAction.X };
            for(int i=0; i<skills.Count; ++i)
                items.Add(skills[i].makeMenuItem(actions[i]));
            MenuPanel result = new MenuPanel(rawStats.name, items);
            return result;
        }

        public void selectSkill(int n)
        {
            selectedSkill = skills[n];
        }
    }
}
