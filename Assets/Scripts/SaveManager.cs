using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    Product product = new Product();
    Product deserializedProduct = new Product();
    void Start()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "save.json");

        product.name = "Apple";
        product.description = "a red color, small, with a green stem";
        product.type = "Fruit";
        string output = JsonConvert.SerializeObject(product);

        //Check if the save exists, otherwise create an empty file at savePath location
        if (!File.Exists(savePath))
        {
            using (StreamWriter sw = File.CreateText(savePath)) ;
        }

        // Write to savePath location for output
        using (StreamWriter sw = new StreamWriter(savePath))
        {
            sw.Write(output);
        }

        //Read the file at savePath and deserialize json
        using (StreamReader sw = File.OpenText(savePath))
        {
            string fileOutput = sw.ReadToEnd();
            deserializedProduct = JsonConvert.DeserializeObject<Product>(fileOutput);
        }

        print(deserializedProduct.name);

        print(Application.persistentDataPath);
    }

    // Define Product (temp for now)
    public class Product
    {
        public string name;
        public string description;
        public string type;
    }
}
