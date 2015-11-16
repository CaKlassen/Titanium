using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium
{
    /// <summary>
    /// Defines an action that is designated by some set of buttons and/or keys.
    /// 
    /// The way actions work is that you define a set of buttons and keys that trigger the action. You can
    /// then evaluate the action against an InputState which will test to see if any of the buttons or keys
    /// are pressed by a player. You can also set a flag that indicates if the action only occurs once when
    /// the buttons/keys are first pressed or whether the action should occur each frame.
    /// 
    /// Using this InputAction class means that you can configure new actions based on keys and buttons
    /// without having to directly modify the InputState type. This means more customization by your games
    /// without having to change the core classes of Game State Management.
    /// </summary>
    public class InputAction
    {
        private readonly Buttons[] buttons;
        private readonly Keys[] keys;
        private readonly bool newPressOnly;
        static ContentManager content;
        // These delegate types map to the methods on InputState. We use these to simplify the evalute method
        // by allowing us to map the appropriate delegates and invoke them, rather than having two separate code paths.
        private delegate bool ButtonPress(Buttons button, PlayerIndex? controllingPlayer, out PlayerIndex player);
        private delegate bool KeyPress(Keys key, PlayerIndex? controllingPlayer, out PlayerIndex player);

        public static InputAction A, B, X, Y;
        public static InputAction LB, RB, LT, RT;
        public static InputAction START, SELECT;
        public static InputAction UP, DOWN, LEFT, RIGHT;
        public static InputAction RSUP, RSDOWN, RSLEFT, RSRIGHT;
        public static InputAction LCLICK, RCLICK;
        
        static InputAction()
        {
            A = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.A },
                true
            );
            B = new InputAction(
                new Buttons[] { Buttons.B },
                new Keys[] { Keys.B },
                true
            );
            X = new InputAction(
                new Buttons[] { Buttons.X },
                new Keys[] { Keys.X },
                true
            );
            Y = new InputAction(
                new Buttons[] { Buttons.Y },
                new Keys[] { Keys.Y },
                true
            );
            LB = new InputAction(
                new Buttons[] { Buttons.LeftShoulder },
                new Keys[] { Keys.D1 },
                true
            );
            RB = new InputAction(
                new Buttons[] { Buttons.RightShoulder },
                new Keys[] { Keys.D2 },
                true
            );
            LT = new InputAction(
                new Buttons[] { Buttons.LeftTrigger },
                new Keys[] { Keys.Q },
                true
            );
            RT = new InputAction(
                new Buttons[] { Buttons.RightTrigger },
                new Keys[] { Keys.W },
                true
            );
            UP = new InputAction(
                new Buttons[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new Keys[] { Keys.Up },
                true
            );
            DOWN = new InputAction(
                new Buttons[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new Keys[] { Keys.Down },
                true
            );
            LEFT = new InputAction(
                new Buttons[] { Buttons.DPadLeft, Buttons.LeftThumbstickLeft },
                new Keys[] { Keys.Left },
                true
            );
            RIGHT = new InputAction(
                new Buttons[] { Buttons.DPadRight, Buttons.LeftThumbstickRight },
                new Keys[] { Keys.Right },
                true
            );
            RSUP = new InputAction(
                new Buttons[] { Buttons.RightThumbstickUp },
                new Keys[] { Keys.NumPad8 },
                true
            );
            RSDOWN = new InputAction(
                new Buttons[] { Buttons.RightThumbstickDown },
                new Keys[] { Keys.NumPad2 },
                true
            );
            RSLEFT = new InputAction(
                new Buttons[] { Buttons.RightThumbstickLeft },
                new Keys[] { Keys.NumPad4 },
                true
            );
            RSRIGHT = new InputAction(
                new Buttons[] { Buttons.RightThumbstickRight },
                new Keys[] { Keys.NumPad6 },
                true
            );
            START = new InputAction(
                new Buttons[] { Buttons.Start },
                new Keys[] { Keys.Enter },
                true
            );
            SELECT = new InputAction(
                new Buttons[] { Buttons.Back },
                new Keys[] { Keys.Escape },
                true
            );
            RCLICK = new InputAction(
                new Buttons[] { Buttons.RightStick },
                new Keys[] { Keys.OemPeriod },
                true
            );
            LCLICK = new InputAction(
                new Buttons[] { Buttons.LeftStick },
                new Keys[] { Keys.NumPad0 },
                true
            );
        }

        /// <summary>
        /// Initializes a new InputAction.
        /// </summary>
        /// <param name="buttons">An array of buttons that can trigger the action.</param>
        /// <param name="keys">An array of keys that can trigger the action.</param>
        /// <param name="newPressOnly">Whether the action only occurs on the first press of one of the buttons/keys, 
        /// false if it occurs each frame one of the buttons/keys is down.</param>
        public InputAction(Buttons[] buttons, Keys[] keys, bool newPressOnly)
        {
            // Store the buttons and keys. If the arrays are null, we create a 0 length array so we don't
            // have to do null checks in the Evaluate method
            this.buttons = buttons != null ? buttons.Clone() as Buttons[] : new Buttons[0];
            this.keys = keys != null ? keys.Clone() as Keys[] : new Keys[0];

            this.newPressOnly = newPressOnly;
        }

        /// <summary>
        /// Evaluates the action against a given InputState.
        /// </summary>
        /// <param name="state">The InputState to test for the action.</param>
        /// <param name="controllingPlayer">The player to test, or null to allow any player.</param>
        /// <param name="player">If controllingPlayer is null, this is the player that performed the action.</param>
        /// <returns>True if the action occurred, false otherwise.</returns>
        public bool Evaluate(InputState state, PlayerIndex? controllingPlayer, out PlayerIndex player)
        {
            // Figure out which delegate methods to map from the state which takes care of our "newPressOnly" logic
            ButtonPress buttonTest;
            KeyPress keyTest;
            if (newPressOnly)
            {
                buttonTest = state.IsNewButtonPress;
                keyTest = state.IsNewKeyPress;
            }
            else
            {
                buttonTest = state.IsButtonPressed;
                keyTest = state.IsKeyPressed;
            }

            // Now we simply need to invoke the appropriate methods for each button and key in our collections
            foreach (Buttons button in buttons)
            {
                if (buttonTest(button, controllingPlayer, out player))
                    return true;
            }
            foreach (Keys key in keys)
            {
                if (keyTest(key, controllingPlayer, out player))
                    return true;
            }

            // If we got here, the action is not matched
            player = PlayerIndex.One;
            return false;
        }

        
        public Buttons getBtn(int i)
        {
            if (i < buttons.Length)
                return buttons[i];
            else
                return 0;
        }

        public bool wasPressed(InputState state)
        {
            PlayerIndex player;
            return Evaluate(state, null, out player);
        }

        public static void Load(ContentManager c)
        {
            content = c;
        }

        public static Texture2D GetIcon(InputAction action, int i)
        {
            Buttons btn = action.getBtn(i);
            switch (btn)
            {
                case Buttons.A:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Face-A");
                case Buttons.B:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Face-B");
                case Buttons.X:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Face-X");
                case Buttons.Y:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Face-Y");
                case Buttons.LeftShoulder:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Shoulder-Left");
                case Buttons.RightShoulder:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Shoulder-Right");
                case Buttons.LeftTrigger:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Trigger-Left");
                case Buttons.RightTrigger:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Trigger-Right");
                case Buttons.DPadUp:
                    return content.Load<Texture2D>("ButtonIcons/HUD-DPad-Up");
                case Buttons.DPadDown:
                    return content.Load<Texture2D>("ButtonIcons/HUD-DPad-Down");
                case Buttons.DPadLeft:
                    return content.Load<Texture2D>("ButtonIcons/HUD-DPad-Left");
                case Buttons.DPadRight:
                    return content.Load<Texture2D>("ButtonIcons/HUD-DPad-Right");
                case Buttons.Start:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Oval-Start");
                case Buttons.Back:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Oval-Back");
                default:
                    return content.Load<Texture2D>("ButtonIcons/HUD-Face-A");
            }
        }

        public Texture2D icon()
        {
            return GetIcon(this, 0);
        }

        public Texture2D icon(int i)
        {
            return GetIcon(this, i);
        }

    }
    
}
