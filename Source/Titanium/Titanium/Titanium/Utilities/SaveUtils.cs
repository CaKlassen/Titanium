﻿using Microsoft.Xna.Framework;
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
        private static string CONTAINER_NAME = "Staged!";

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

        /// <summary>
        /// checks to see if storage device is registered.
        /// </summary>
        /// <returns>true if registered; false otherwise</returns>
        public bool storageRegistered()
        {
            if (storageDevice == null)
                return false;
            else
                return true;
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
                IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = storageDevice.EndOpenContainer(result);

                if(container.FileExists(SAVE_FILE))//if that file already exists
                {
                    container.DeleteFile(SAVE_FILE);//delete existing file
                }

                Stream stream = container.CreateFile(SAVE_FILE);//create file

                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(stream, data);

                stream.Close();//close the file
                container.Dispose();//disposing container commits changes to device
            }
        }

        /// <summary>
        /// Saves the highscores.
        /// </summary>
        /// <param name="HighScore">list of </param>
        public void SaveXboxHighScore(int[] HighScore)
        {
            HighscoreData highscores = new HighscoreData();
            highscores.highscores = HighScore;


            if (storageDevice != null && storageDevice.IsConnected)
            {
                IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = storageDevice.EndOpenContainer(result);

                if (container.FileExists(HIGHSCORE_FILE))//if that file already exists
                {
                    container.DeleteFile(HIGHSCORE_FILE);//delete existing file
                }

                Stream stream = container.CreateFile(HIGHSCORE_FILE);//create file

                XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                serializer.Serialize(stream, highscores);

                stream.Close();//close the file
                container.Dispose();//disposing container commits changes to device
            }
        }


        public SaveData loadGame()
        {
            SaveData data;
#if WINDOWS
            data = loadWindows();
#else
            data = loadXbox();
#endif
            return data;
        }

        /// <summary>
        /// checks if the Save Data file exists.
        /// </summary>
        /// <returns>true if the file exists; false otherwise.</returns>
        public bool CheckFileExists()
        {
            bool exists = false;
#if WINDOWS
            exists = CheckFileExistsWindows();
#else
            exists = CheckFileExistsXbox();
#endif
            return exists;
        }

        public SaveData loadWindows()
        {
            //temporary; here so it builds
            SaveData data = new SaveData();
            return data;
        }

        /// <summary>
        /// loads the save file.
        /// </summary>
        /// <returns>SaveData struct containing the save data from file.</returns>
        public SaveData loadXbox()
        {
            SaveData loadData;

            IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = storageDevice.EndOpenContainer(result);

            result.AsyncWaitHandle.Close();

            Stream stream = container.OpenFile(SAVE_FILE, FileMode.Open);//open file
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            loadData = (SaveData)serializer.Deserialize(stream);

            stream.Close();
            container.Dispose();

            return loadData;
        }

        private bool CheckFileExistsWindows()
        {
            return false;
        }

        private bool CheckFileExistsXbox()
        {
            IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = storageDevice.EndOpenContainer(result);

            return container.FileExists(SAVE_FILE);
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
