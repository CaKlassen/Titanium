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
        LEVEL9,
        BATTLE_FIRST,
        BATTLE_BOSS,
        END_WIN,
        END_LOSE
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

                case ConversationType.BATTLE_FIRST:
                {
                    return makeBattleFirst();
                }

                case ConversationType.BATTLE_BOSS:
                {
                    return makeBattleBoss();
                }

                case ConversationType.END_WIN:
                {
                    return makeEndWin();
                }

                case ConversationType.END_LOSE:
                {
                    return makeEndLose();
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
            c.addTextbox(new Textbox("Zzz...Mommy...", TextChar.LEO));
            c.addTextbox(new Textbox("Rrghhhh...", TextChar.CLEM));
            c.addTextbox(new Textbox("Ugh. My head.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Where am I?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Good lord, a thief!", TextChar.LEO));
            c.addTextbox(new Textbox("Who, me?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Stay back, fiend! If you harm me, my father will have your head!", TextChar.LEO));
            c.addTextbox(new Textbox("Lovely. I'm lost with a prince.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("That's Prince Leonard to you, scoundrel.", TextChar.LEO));
            c.addTextbox(new Textbox("Fantastic. Hey prince, I think we have a bigger concern.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Good lord, a beast!", TextChar.LEO));
            c.addTextbox(new Textbox("Clementine not beast.", TextChar.CLEM));
            c.addTextbox(new Textbox("Ugh, it speaks.", TextChar.LEO));
            c.addTextbox(new Textbox("Little prince should watch its tongue.", TextChar.CLEM));
            c.addTextbox(new Textbox("I like you already, wolf-lady.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Greetings, friends! So good to see you alive and well.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("What new tragedy befalls me now?", TextChar.LEO));
            c.addTextbox(new Textbox("Ooh, tragedy. I like the sound of that!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("I have gathered the three of you here today so that we may play... a game!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("A game? Where are we?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("More importantly, where are YOU?", TextChar.LEO));
            c.addTextbox(new Textbox("Now now, it wouldn't be much fun if I spoiled the surprise.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Listen carefully, friends. Before you lies a sprawling dungeon, filled with monsters.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("On each floor of this dungeon, your task is to slay every monster that you come across.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Once you have done so, a trap door will open that will allow you to progress.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("And what if we decide not to play your... game?", TextChar.LEO));
            c.addTextbox(new Textbox("You will die.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Clementine want to rip your head from shoulders.", TextChar.CLEM));
            c.addTextbox(new Textbox("That's the spirit! Now, off with you!", TextChar.VILLAIN));

            return c;
        }

        private static Conversation makeLevel2()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Excellent progress, friends! You make such a great team!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Shut up.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Your manners could use some work, however.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Good sir, might I trouble you for a glass of water? My throat is parched.", TextChar.LEO));
            c.addTextbox(new Textbox("Water? Oh no, there's none of that here. I hear the cave walls have a lovely dampness this time of year.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("That reminds me, though. Have you noticed the bottle of glistening, succulent liquid on each floor?", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Those are what I like to call 'health potions'! If you're feeling down and out, try a sip!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Clementine not trust man of big words.", TextChar.CLEM));
            c.addTextbox(new Textbox("Trust me, trust me not. When you're clinging to life, the choice is yours.", TextChar.VILLAIN));

            return c;
        }

        private static Conversation makeLevel3()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Is there no end to these tunnels?", TextChar.LEO));
            c.addTextbox(new Textbox("Save your whining. You're going to need your energy if we're going to survive.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Oh, reliant on me are we? I understand completely! Every team needs a strong leader!", TextChar.LEO));
            c.addTextbox(new Textbox("Clementine did not vote prince man for leader. Clem not need leader.", TextChar.CLEM));
            c.addTextbox(new Textbox("Now now, my dear wolf... thing. This is not the time for insubordination!", TextChar.LEO));
            c.addTextbox(new Textbox("He's right, Clementine. Wait until we escape. THEN, you can kill and eat him.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Clementine will wait.", TextChar.CLEM));
            c.addTextbox(new Textbox("Fantastic jokes! Humour truly is the best way to keep our spirits up!", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel4()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Clementine smells... wet.", TextChar.CLEM));
            c.addTextbox(new Textbox("Where are we now?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("The royal waterways! Never before has human excrement been transported so gracefully!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Wait, we're beneath the capital? My father will have your head when he finds out about this!", TextChar.LEO));
            c.addTextbox(new Textbox("Oh, hush. I've simply repurposed part of the sewers for my game. Where's the harm in that?", TextChar.VILLAIN));
            c.addTextbox(new Textbox("...You may want to watch your step, however. The ground is... sharp in places.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("I can't believe this. This... This is vandalism! My father's sewers have been ruined!", TextChar.LEO));
            c.addTextbox(new Textbox("I hate to break it to you, prince, but this place is CLEANER than most of the waterways.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I've spent many years travelling through the sewers. The green slime is everywhere. Your father doesn't care one bit.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I... I can't believe...", TextChar.LEO));
            c.addTextbox(new Textbox("That your glorious father might have his flaws? Shocker.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I can't believe you use these waterways for... for travelling! This is crown property!", TextChar.LEO));
            c.addTextbox(new Textbox("For a moment, I thought you'd achieved understanding. Oh well.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeLevel5()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("What's that glowing?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("That, friends, is what I like to call a Mystery Box!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Awesome! I'll be steering clear of that.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I am in full agreement.", TextChar.LEO));
            c.addTextbox(new Textbox("Don't be so hasty! My Mystery Box is a marvel of modern technology.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Open it, and you may find yourself rewarded for your courage...", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... Or, you may find your pride... Injured. Hee hee!", TextChar.VILLAIN));

            return c;
        }

        private static Conversation makeLevel6()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Can... Can we take a moment to rest?", TextChar.LEO));
            c.addTextbox(new Textbox("Clementine not need rest.", TextChar.CLEM));
            c.addTextbox(new Textbox("Perhaps not, but we are not all freakishly large, anthropomorphic wolves.", TextChar.LEO));
            c.addTextbox(new Textbox("Clementine not a freak.", TextChar.CLEM));
            c.addTextbox(new Textbox("Enough, you two. We'll take a moment to rest. Is there a rule against that, O voice in the sky?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Not at all! Rest away. The sludgers like it when their prey is stationary.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... Thanks for that.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeLevel7()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Now THIS, I could get used to.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Surely, you jest? These hallways are better suited to a criminal than a prince!", TextChar.LEO));
            c.addTextbox(new Textbox("... Oh, I see.", TextChar.LEO));
            c.addTextbox(new Textbox("I'm positively thrilled to see you so at home in my lodgings!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("I haven't had visitors in many a year.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Clementine cleans better than you. Clementine no get visitors.", TextChar.CLEM));
            c.addTextbox(new Textbox("All part of the appeal! I can't wait for you to meet my spiders.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... Spiders?", TextChar.LEO));
            c.addTextbox(new Textbox("Of course! My family is quite large. I'm sure you'll get along well!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("I hate spiders.", TextChar.LEO));

            return c;
        }

        private static Conversation makeLevel8()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Clementine liked sewers better.", TextChar.CLEM));
            c.addTextbox(new Textbox("Shockingly, I feel the same. This house is positively ramshackle.", TextChar.LEO));
            c.addTextbox(new Textbox("We must be getting close. How big could this house be?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Father says that some of the abandoned buildings by the water once belonged to barons.", TextChar.LEO));
            c.addTextbox(new Textbox("Your point being?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I suppose you've never seen the inside of a mansion. They're quite large.", TextChar.LEO));
            c.addTextbox(new Textbox("Fantastic.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeLevel9()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Congratulations are in order, friends!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Did we make it? Are we finally free?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Of course not!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Oh.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("How could I let you leave without your final test?", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Clementine not like tests. Clementine not know how to read.", TextChar.CLEM));
            c.addTextbox(new Textbox("And what kind of test might this be, fiend?", TextChar.LEO));
            c.addTextbox(new Textbox("You have proven yourself more than capable of surviving my dungeon, friends...", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... You have slain countless monsters, and have triumphed over my dastardly traps...", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... But before you escape this place, there is one last challenge for you to overcome...", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... Me.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("...", TextChar.LEO));
            c.addTextbox(new Textbox("I eagerly await your audience.", TextChar.VILLAIN));

            return c;
        }

        private static Conversation makeBattleFirst()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Are those... bats?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Good. Clementine was hungry.", TextChar.CLEM));
            c.addTextbox(new Textbox("Repulsive creature.", TextChar.LEO));
            c.addTextbox(new Textbox("That's not very nice. They're just bats.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I was not referring to the bats.", TextChar.LEO));
            c.addTextbox(new Textbox("Let us slay these foul beasts without further delay. Have you mettle, display it now.", TextChar.LEO));
            c.addTextbox(new Textbox("If you say so, prince.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeBattleBoss()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("My God...", TextChar.LEO));
            c.addTextbox(new Textbox("I can't say that I was expecting... this.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Clementine not like feeling small.", TextChar.CLEM));
            c.addTextbox(new Textbox("Take it in slowly, friends.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("It's... It's hard not to. You stink.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("What ARE you?", TextChar.LEO));
            c.addTextbox(new Textbox("I see that your manners have failed to improve. No matter.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("By the time that I finish with you, you will have no need of manners!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("...", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Because you'll be dead.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("Did that come through clear enough? It sounded better in my head.", TextChar.VILLAIN));
            c.addTextbox(new Textbox("This guy's a nut.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("I couldn't agree more.", TextChar.LEO));
            c.addTextbox(new Textbox("Clementine want kill this thing.", TextChar.CLEM));
            c.addTextbox(new Textbox("Finally, something for us to unite over.", TextChar.LEO));
            c.addTextbox(new Textbox("Let's kill this freak.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeEndWin()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("Fresh air at last!", TextChar.LEO));
            c.addTextbox(new Textbox("Depends on your definition of 'fresh', I suppose.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Where is your enthusiasm, sir? We have bested our foe!", TextChar.LEO));
            c.addTextbox(new Textbox("The bards will sing tales of my triumphs for generations to come!", TextChar.LEO));
            c.addTextbox(new Textbox("YOUR triumphs?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Of course! Without my leadership, we would surely have perished.", TextChar.LEO));
            c.addTextbox(new Textbox("I see...", TextChar.KLEPTO));
            c.addTextbox(new Textbox("But worry not, my conniving companion. I shall not forget your efforts.", TextChar.LEO));
            c.addTextbox(new Textbox("... That being said, there is some final business to attend to...", TextChar.LEO));
            c.addTextbox(new Textbox("I can barely contain my excitement.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("You see, there is but one problem. During our misadventures, you both committed a crime...", TextChar.LEO));
            c.addTextbox(new Textbox("A most dastardly, unforgivable crime...", TextChar.LEO));
            c.addTextbox(new Textbox("You have trespassed in my father's royal waterways!", TextChar.LEO));
            c.addTextbox(new Textbox("Give me a break.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("For this crime, you will be punished accordingly. A tribunal of three will decide your f--", TextChar.LEO));
            c.addTextbox(new Textbox("Hey, Clementine?", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Clementine listens.", TextChar.CLEM));
            c.addTextbox(new Textbox("You're free to eat the small prince now.", TextChar.KLEPTO));
            c.addTextbox(new Textbox("Clementine has long awaited this moment.", TextChar.CLEM));
            c.addTextbox(new Textbox("Wait, what? You can't do this, I'm a PRINCE!", TextChar.LEO));
            c.addTextbox(new Textbox("No! Stop! NOOOOO--", TextChar.LEO));
            c.addTextbox(new Textbox("...", TextChar.KLEPTO));
            c.addTextbox(new Textbox("...", TextChar.KLEPTO));
            c.addTextbox(new Textbox("... Much better.", TextChar.KLEPTO));

            return c;
        }

        private static Conversation makeEndLose()
        {
            Conversation c = new Conversation();
            c.addTextbox(new Textbox("... No... Not like this...", TextChar.LEO));
            c.addTextbox(new Textbox("Hee hee! It seems your performance has come to an untimely end!", TextChar.VILLAIN));
            c.addTextbox(new Textbox("... But what a MARVELOUS performance it was...", TextChar.VILLAIN));

            return c;
        }
    }
}
