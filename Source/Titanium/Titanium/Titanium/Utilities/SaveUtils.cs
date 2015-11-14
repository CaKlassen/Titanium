using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Utilities
{
    public struct SaveData
    {
        public long levelSeed;
        public int level;
        public int score;
        public int[] partyHealth;
    }


    public class SaveUtils
    {
        private static SaveUtils instance;
        private static string SAVE_FILE = "savegame.sav";
        private static string HIGHSCORE_FILE = "highscores.sav";

        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,

            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
        }
        
        public struct HighscoreData
        {
            public int[] highscores;
        }

        private StorageDevice storageDevice;
        private SavingState savingState = SavingState.NotSaving;
        private IAsyncResult asyncResult;
        private PlayerIndex playerIndex = PlayerIndex.One;
        private StorageContainer storageContainer;
        

        public SaveUtils()
        {
#if XBOX
            BaseGame.instance.Components.Add(new GamerServicesComponent(BaseGame.instance));
#endif
        }


        public void saveGame(SaveData data)
        {
#if WINDOWS
            saveWindows(data);
#else
            saveXbox(data);
#endif
        }

        public void saveWindows(SaveData data)
        {

        }


        public void saveXbox(SaveData data)
        {

        }


        public void loadGame()
        {
#if WINDOWS
            loadWindows();
#else
            loadXbox();
#endif
        }

        public void loadWindows()
        {

        }

        public void loadXbox()
        {

        }

        public static SaveUtils getInstance()
        {
            if (instance == null)
            {
                instance = new SaveUtils();
            }

            return instance;
        }
    }
}
