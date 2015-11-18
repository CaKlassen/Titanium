﻿using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Titanium.Utilities
{
    public static class SoundUtils
    {
        public enum Music
        {
            TitleTheme,
            ArenaTheme,
            BattleTheme
        }
        static string musicBaseDir = "Sound/bgm/";
        static string[] musicDirs = { "title", "arena", "battle"};
        static List<Song> music = new List<Song>();
        static Song currentSong;
        public static bool IsFading = false;
        static bool fadeOut;
        public static float volume = 0.2f;

        public enum Sound
        {
            Success,
            Failure,
            InputSelection,
            Hit
        }
        static string soundBaseDir = "Sound/sfx/";
        static string[] soundDirs = { };
        static List<SoundEffect> sounds = new List<SoundEffect>();
        


        public static void Load(ContentManager content)
        {
            foreach(string dir in musicDirs)
                music.Add(content.Load<Song>(musicBaseDir + dir));

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0;
        }

        public static void FadeIn(Music song)
        {
            MediaPlayer.Volume = 0;
            MediaPlayer.Play(music[(int)song]);
            IsFading = true;
            fadeOut = false;
        }

        public static void FadeOut()
        {
            IsFading = true;
            fadeOut = true;
        }

        public static void Update()
        {
            if(fadeOut)
            {
                if (MediaPlayer.Volume <= 0)
                {

                    fadeOut = false;
                    MediaPlayer.Stop();
                }
                    
                else
                    MediaPlayer.Volume -= 0.005f;
            }
            else
            {
                if (MediaPlayer.Volume >= volume)
                {
                    IsFading = false;
                    fadeOut = true;
                }
                else
                    MediaPlayer.Volume += 0.005f;
            }
        }
    }
}
