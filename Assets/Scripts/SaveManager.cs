using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    Data client_data = new Data();
    Data deserialized_data = new Data();
    void Start()
    {
        string data_path = Path.Combine(Application.persistentDataPath, "client_data.json");

        client_data.settings.volume = 5.5f;

        string output = JsonConvert.SerializeObject(client_data, Formatting.Indented);

        //Check if the save exists, otherwise create an empty file at savePath location
        if (!File.Exists(data_path))
        {
            using (StreamWriter sw = File.CreateText(data_path)) ;
        }

        // Write to savePath location for output
        using (StreamWriter sw = new StreamWriter(data_path))
        {
            sw.Write(output);
        }

        //Read the file at savePath and deserialize json
        using (StreamReader sw = File.OpenText(data_path))
        {
            string fileOutput = sw.ReadToEnd();
            deserialized_data = JsonConvert.DeserializeObject<Data>(fileOutput);
        }

        print(deserialized_data.settings.volume);

        print(Application.persistentDataPath);
    }

    // Define client data (temp for now)
    public class Data
    {
        public Settings settings = new Settings();
        public class Settings
        {
            public float volume;
            public string sensitivity;
        }
    }
}
