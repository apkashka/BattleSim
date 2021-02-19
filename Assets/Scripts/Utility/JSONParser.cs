using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class JSONParser
{
    public T ParseFile<T>(string path) where T: class
    {
        TextAsset targetFile = Resources.Load<TextAsset>(path);
        var gameData = JsonConvert.DeserializeObject<T>(targetFile.text);
        return gameData;
    }
}
