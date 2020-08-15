using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Newtonsoft.Json;

namespace IPcamera
{
    static class SaveCameras
    {
        public static string saveDataPath = AppDomain.CurrentDomain.BaseDirectory + "settings.json";

        /// <summary>
        /// Creates a new SaveDataObject and serializes it to a JSON file.
        /// </summary>
        public static void Save()
        {
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(saveDataPath))
                using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, new SaveDataObject(SaveData.cameras, SaveData.gridWidth, SaveData.gridHeight));
            }
        }

        /// <summary>
        /// Deserializes a JSON file to a SaveDataObject and stores the contents in SaveData where it distributes the data to the correct places.
        /// </summary>
        public static void Load()
        {
            // Deserialize json data and put it in a SaveDataObject
            SaveDataObject saveDataObject;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamReader sr = new StreamReader(saveDataPath))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                saveDataObject = (SaveDataObject)serializer.Deserialize(reader, typeof(SaveDataObject));
            }

            // Converting SimpleCameras to actual Camera objects, SimpleCamera is used to ensure JSON file isn't a mess
            SaveData.cameras = new List<Camera>();
            for (int i = 0; i < saveDataObject.cameras.Count; i++)
            {
                SaveData.cameras.Add(new Camera(saveDataObject.cameras[i].address, 
                                                saveDataObject.cameras[i].username,
                                                saveDataObject.cameras[i].password,
                                                saveDataObject.cameras[i].onvifAddress));
            }

            SaveData.gridWidth = saveDataObject.gridWidth;
            SaveData.gridHeight = saveDataObject.gridHeight;
        }
    }

    /// <summary>
    /// This class will be used to save it's contents to a JSON file.
    /// </summary>
    class SaveDataObject
    {
        public List<SimpleCamera> cameras;
        public int gridWidth;
        public int gridHeight;

        public SaveDataObject(List<Camera> cameras, int gridWidth, int gridHeight)
        {
            this.cameras = new List<SimpleCamera>();

            // Converts Cameras to SimpleCameras for saving.
            if (cameras != null)
            {
                for (int i = 0; i < cameras.Count; i++)
                {
                    this.cameras.Add(cameras[i].GetSimpleCamera());
                }
            }

            this.gridWidth = gridWidth;
            this.gridHeight = gridHeight;
        }
    }

    /// <summary>
    /// This class sole purpose is to allow Cameras to easily be saved and loaded from a file without all the unnecessary junk regular Cameras comes with.
    /// </summary>
    class SimpleCamera
    {
        public string address;
        public string username;
        public string password;
        public string onvifAddress;

        public SimpleCamera(string address, string username, string password, string onvifAddress)
        {
            this.address = address;
            this.username = username;
            this.password = password;
            this.onvifAddress = onvifAddress;
        }

        public SimpleCamera(SimpleCamera simpleCamera)
        {
            address = simpleCamera.address;
            username = simpleCamera.username;
            password = simpleCamera.password;
            onvifAddress = simpleCamera.onvifAddress;
        }

    }

    /// <summary>
    /// This class is used to grab data that is going to be saved and loaded from around the code base.
    /// </summary>
    static class SaveData
    {

        public static List<Camera> cameras;
        public static int gridWidth = 0;
        public static int gridHeight = 0;
    }
}
