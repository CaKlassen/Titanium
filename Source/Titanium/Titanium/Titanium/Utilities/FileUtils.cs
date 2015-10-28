using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Titanium.Battle;
using Titanium.Entities;

namespace Titanium.Utilities
{
    class FileUtils
    {
        public FileUtils()
        {

        }

        public List<UnitStats> FileToSprite(String path)
        {
            List<UnitStats> result = new List<UnitStats>();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(path);
            while((line = file.ReadLine())!=null)
            {
                char[] delimiters = new char[] { ',' };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                UnitStats u = new UnitStats();
                u.init(parts);
                result.Add(u);
            }
            return result;
        }

        public Sprite CreateNewSprite(String path)
        {
            Sprite result = new Sprite();
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader("Content/Sprites/"+path);
            while ((line = file.ReadLine()) != null)
            {
                char[] delimiters = new char[] { ',' };
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                UnitStats u = new UnitStats();
                u.init(parts);
                if(u.name.CompareTo(path)==0)
                {
                    result.setParam(u, 0, 0);
                    return result;
                }
            }
            return result;
        }
    }
}
