using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
        Object stateObject;
        
        /// <summary>
        /// constructor
        /// </summary>
        public SaveUtils()
        {
#if XBOX360
            BaseGame.instance.Components.Add(new GamerServicesComponent(BaseGame.instance));
#endif
        }


        public void RegisterStorage()
        {
            //Get StorageDevice
            if (!Guide.IsVisible)
            {
                storageDevice = null;//reset device                
                asyncResult = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);//storage device selected
                storageDevice = StorageDevice.EndShowSelector(asyncResult);//set storage device
            }
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

        /// <summary>
        /// Saves data to the Xbox360
        /// </summary>
        /// <param name="data"></param>
        public void saveXbox(SaveData data)
        {
            if (storageDevice != null && storageDevice.IsConnected)
            {
                IAsyncResult result = storageDevice.BeginOpenContainer("Titanium", null, null);
                result.AsyncWaitHandle.WaitOne();
                storageContainer = storageDevice.EndOpenContainer(result);

                if(storageContainer.FileExists(SAVE_FILE))//if that file already exists
                {
                    storageContainer.DeleteFile(SAVE_FILE);//delete existing file
                }

                Stream stream = storageContainer.CreateFile(SAVE_FILE);//create file

                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(stream, data);

                stream.Close();//close the file
                storageContainer.Dispose();//disposing container commits changes to device
            }
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
