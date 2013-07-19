/*This code is managed under the Apache v2 license. 
To see an overview: 
http://www.tldrlegal.com/license/apache-license-2.0-(apache-2.0)

Author: Robert Gawdzik
www.github.com/rgawdzik/

THIS CODE HAS NO FORM OF ANY WARRANTY, AND IS CONSIDERED AS-IS.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Paradox.Game
{
    public class Config
    {
        public bool MusicEnabled;
        public bool SoundEnabled;
        public bool VoiceEnabled;
        public int Mission;
        public bool SpaceBattle;
        public bool Win;
        public bool End;
        public Config()
        {
            MusicEnabled = true;
            SoundEnabled = true;
            VoiceEnabled = true;
            Mission = 0;
            SpaceBattle = false;
            Win = false;
            End = false;
        }
    }

    public class Posting
    {
        public string Name { get; set; }
        public int Score { get; set; }

        public Posting()
        {
            Name = "";
            Score = 0;
        }

        public Posting(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }

    public class Save
    {
        const string fileName = "save.xml";
        public Config Config;
        public List<Posting> Leaderboards;
        public Save()
        {
            Config = new Config();
            Leaderboards = new List<Posting>();
            Leaderboards.Add(new Posting("n00b", 3));
            Leaderboards.Add(new Posting("normal", 10));
            Leaderboards.Add(new Posting("pro", 30));
            Leaderboards.Add(new Posting("zealot", 50));
            Leaderboards.Add(new Posting("godly", 75));
        }

        public Save(Config config, List<Posting> leaderboards)
        {
            Config = config;
            Leaderboards = leaderboards;
        }

        public void SaveFile()
        {
            IsolatedStorageFile storage = GetUserStoreAsAppropriateForCurrentPlatform(); //Gets the Isolated Storage location for the application.  It's functionality is as a File System, not an actual File.
            IsolatedStorageFileStream stream = storage.CreateFile(fileName); //Creates a file stream from a newly created xml file that is pathed via the const fileName.
            XmlSerializer xml = new XmlSerializer(GetType()); //Creates a Serializable Object with this current object as a template, via the inherited method from the Object class, GetType().
            xml.Serialize(stream, this); //Serializes the object, passing this current object with the this keyword.
            stream.Close();
            stream.Dispose();
        }

        public static Save LoadFile()
        {
            IsolatedStorageFile storage = GetUserStoreAsAppropriateForCurrentPlatform(); //Gets the Isolated Storage location for the application.
            Save save;

            if (storage.FileExists(fileName)) //Checks if the const fileName exists within Isolated Storage.
            {
                try
                {
                    IsolatedStorageFileStream stream = storage.OpenFile(fileName, FileMode.Open); //Opens the file within Isolated Storage, creating a stream.
                    XmlSerializer xml = new XmlSerializer(typeof(Save)); //Creates a xml serializable object that takes in a Save object, no object instance so typeof() is used to pass a Type param to xml Serializer.
                    save = xml.Deserialize(stream) as Save;  //Deserializes the file, creates a Save object via as keyword.
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception e)
                {
                    save = new Save();
                }
            }
            else
            {
                save = new Save(); //If no file is found, a new one is created.
            }
            return save;
        }

        static IsolatedStorageFile GetUserStoreAsAppropriateForCurrentPlatform()
        {
            #if WINDOWS
            return IsolatedStorageFile.GetUserStoreForDomain();
            #else
            return IsolatedStorageFile.GetUserStoreForApplication();
            #endif
        }
    }
}
