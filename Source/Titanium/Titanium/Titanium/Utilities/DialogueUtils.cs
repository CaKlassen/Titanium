using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Entities;

namespace Titanium.Utilities
{
    public enum ConversationType
    {
        LEVEL1,
        LEVEL2,
        LEVEL3,
        LEVEL4,
        LEVEL5,
        LEVEL6,
        LEVEL7,
        LEVEL8,
        LEVEL9
    }

    public static class DialogueUtils
    {
        public static Conversation makeConversation(ConversationType conversationTag)
        {
            switch (conversationTag)
            {
                case ConversationType.LEVEL1:
                {
                    return makeLevel1();
                }

                case ConversationType.LEVEL2:
                {
                    return makeLevel2();
                }

                case ConversationType.LEVEL3:
                {
                    return makeLevel3();
                }

                case ConversationType.LEVEL4:
                {
                    return makeLevel4();
                }

                case ConversationType.LEVEL5:
                {
                    return makeLevel5();
                }

                case ConversationType.LEVEL6:
                {
                    return makeLevel6();
                }

                case ConversationType.LEVEL7:
                {
                    return makeLevel7();
                }

                case ConversationType.LEVEL8:
                {
                    return makeLevel8();
                }

                case ConversationType.LEVEL9:
                {
                    return makeLevel9();
                }

                default:
                {
                    return null;
                }
            }
        }


        // CONVERSATIONS //

        private static Conversation makeLevel1()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Mommy...", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel2()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel3()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel4()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel5()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel6()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel7()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel8()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel9()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("WOAH", TextChar.LEO));

            return c;
        }
    }
}
