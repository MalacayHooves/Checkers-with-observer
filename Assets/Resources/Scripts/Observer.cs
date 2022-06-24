using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Observer : MonoBehaviour
{
    private static string path;

    private List<string> _dataStrings = new List<string>();

    private void Awake()
    {
        path = Application.dataPath + "/observer.txt";
        FileStream newFile = new FileStream(path, FileMode.OpenOrCreate);
        newFile.Close();
        SaveData("this\n");
        SaveData("is\n");
        SaveData("my\n");
        SaveData("text!");

    }

    private void OnEnable()
    {
        foreach (string element in _dataStrings)
        {
            print(element);
        }
    }

    private static void SaveData(string text)
    {
        using(FileStream stream = File.OpenWrite(path))
        {
            using(BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.Default, false))
            {
                writer.Write(text);
            }
        }
    }

    private void ReadData()
    {
        _dataStrings.Clear();
        _dataStrings.TrimExcess();

        FileStream stream = File.OpenRead(path);

        BinaryReader binaryData = new BinaryReader(stream);
        
        while(binaryData.PeekChar() != -1)
        {
            _dataStrings.Add(binaryData.ReadString());
        }
    }

    //public static event ClickEventHandler OnClickEventHandler;
    //public delegate void ClickEventHandler(BaseClickComponent component);
}
