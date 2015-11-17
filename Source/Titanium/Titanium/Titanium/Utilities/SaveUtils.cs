using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Titanium.Entities;

namespace Titanium.Utilities
{
    public struct SaveData
    {
        public int seed;
        public int level;
        public int score;
        public int[] partyHealth;
    }

    public struct HighscoreData
    {
        public List<int> highscores;
    }

    public class SaveUtils
    {
        private static SaveUtils instance;
        private static string SAVE_FILE = "savegame.sav";
        private static string HIGHSCORE_FILE = "highscores.sav";
        private static string CONTAINER_NAME = "Staged!";
        private string completePath;
        private string completePathHighScore;

        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,

            ReadyToOpenStorageContainer,    // once we have a storage device start here
            OpeningStorageContainer,
            ReadyToSave
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
#if WINDOWS
            //create the path to C:\ProgramData
            var systemPath = System.Environment.
                 GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            //append to make a folder for our game; C:\ProgramData\Staged!
            var path = Path.Combine(systemPath, "Staged!");

            //check if it exists; if it doesn't, create it
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //append again to have a path with filename for saving
            completePath = Path.Combine(path, SAVE_FILE);
            completePathHighScore = Path.Combine(path, HIGHSCORE_FILE);
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

        /// <summary>
        /// Regitsters the storage device for Xbox360
        /// </summary>
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

        /// <summary>
        /// Saves the save data.
        /// </summary>
        /// <param name="data">SaveData struct to save.</param>
        public void saveGame(SaveData data)
        {
#if WINDOWS
            saveWindows(data);
#else
            saveXbox(data);
#endif
        }

        /// <summary>
        /// Saves the save data to the PC
        /// </summary>
        /// <param name="data">SaveData struct to save.</param>
        private void saveWindows(SaveData data)
        {
            //if the path isn't null
            if(completePath != null)
            {
                using (var stream = new FileStream(completePath, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                    serializer.Serialize(stream, data);
                }
            }
        }

        /// <summary>
        /// Saves the save data to the Xbox360
        /// </summary>
        /// <param name="data">SaveData struct to save.</param>
        private void saveXbox(SaveData data)
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
        /// <param name="HighScore">list of highscores</param>
        public void saveHighScores(List<int> HighScore)
        {
#if WINDOWS
            SaveWindowsHighScores(HighScore);
#else
            SaveXboxHighScores(HighScore);
#endif
        }

        /// <summary>
        /// Saves the highscores.
        /// </summary>
        /// <param name="HighScore">list of highscores</param>
        public void SaveXboxHighScores(List<int> HighScore)
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

        /// <summary>
        /// Saves the highscores.
        /// </summary>
        /// <param name="HighScore">list of highscores</param>
        public void SaveWindowsHighScores(List<int> HighScore)
        {
            HighscoreData highscores = new HighscoreData();
            highscores.highscores = HighScore;

            //if the path isn't null
            if (completePath != null)
            {
                using (var stream = new FileStream(completePathHighScore, FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                    serializer.Serialize(stream, highscores);
                }
            }
        }

        /// <summary>
        /// loads the game.
        /// </summary>
        /// <returns>SaveData struct containing the save data from file.</returns>
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
            exists = CheckFileExistsWindows(completePath);
#else
            exists = CheckFileExistsXbox(SAVE_FILE);
#endif
            return exists;
        }

        /// <summary>
        /// checks if the HighScore file exists.
        /// </summary>
        /// <returns>true if the file exists; false otherwise.</returns>
        public bool CheckHighScoreExists()
        {
            bool exists = false;
#if WINDOWS
            exists = CheckFileExistsWindows(completePathHighScore);
#else
            exists = CheckFileExistsXbox(HIGHSCORE_FILE);
#endif
            return exists;
        }

        /// <summary>
        /// loads the save file from PC.
        /// </summary>
        /// <returns>SaveData struct containing the save data from file.</returns>
        public SaveData loadWindows()
        {
            SaveData data;

            using(var stream = new FileStream(completePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                data = (SaveData)serializer.Deserialize(stream);
            }
            return data;
        }

        /// <summary>
        /// loads the save file from xbox storage.
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

        /// <summary>
        /// loads the HighScores from file.
        /// </summary>
        /// <returns>the highscore data structure</returns>
        public HighscoreData loadHighScores()
        {
            HighscoreData data;
#if WINDOWS
            data = loadHighScoresWindows(); 
#else
            data = loadHighScoresXbox();
#endif
            return data;
        }

        /// <summary>
        /// load the HighScore file from storage.
        /// </summary>
        /// <returns>the highscore data structure</returns>
        public HighscoreData loadHighScoresXbox()
        {
            HighscoreData loadData;

            IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = storageDevice.EndOpenContainer(result);

            result.AsyncWaitHandle.Close();

            Stream stream = container.OpenFile(HIGHSCORE_FILE, FileMode.Open);//open file
            XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
            loadData = (HighscoreData)serializer.Deserialize(stream);

            stream.Close();
            container.Dispose();

            return loadData;
        }

        /// <summary>
        /// load the HighScore file from PC.
        /// </summary>
        /// <returns>the highscore data structure</returns>
        public HighscoreData loadHighScoresWindows()
        {
            HighscoreData data;

            using (var stream = new FileStream(completePathHighScore, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(HighscoreData));
                data = (HighscoreData)serializer.Deserialize(stream);
            }
            return data;
        }

        /// <summary>
        /// checks if the file already exists on the PC
        /// </summary>
        /// <returns>true if the file exists; false otherwise.</returns>
        private bool CheckFileExistsWindows(string filename)
        {
            return (File.Exists(filename));
        }

        /// <summary>
        /// checks if the file already exists on the xbox
        /// </summary>
        /// <returns>true if it exists; false otherwise.</returns>
        private bool CheckFileExistsXbox(string filename)
        {
            IAsyncResult result = storageDevice.BeginOpenContainer(CONTAINER_NAME, null, null);
            result.AsyncWaitHandle.WaitOne();
            StorageContainer container = storageDevice.EndOpenContainer(result);

            return container.FileExists(filename);
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
