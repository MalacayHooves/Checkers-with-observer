using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Observer : MonoBehaviour, Checkers.IObserver
{
    private static string path;

    private List<string> _dataStrings = new List<string>();

    private void Awake()
    {
        path = Application.dataPath + "/observer.txt";
        FileStream file = new FileStream(path, FileMode.OpenOrCreate);
        file.Close();

        Checkers.Player.OnSaveData += SaveData;
        Checkers.Player.OnClearData += ClearData;
    }

    private void OnDisable()
    {
        Checkers.Player.OnSaveData -= SaveData;
        Checkers.Player.OnClearData -= ClearData;
    }

    private void Start()
    {
        ReadData();
    }

    public void SaveData(string data)
    {
        using(FileStream stream = new FileStream(path, FileMode.Append))
        {
            using(BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.Default, false))
            {
                writer.Write($"{data}\n");
            }
        }
    }

    public void ReadData()
    {
        _dataStrings.Clear();
        _dataStrings.TrimExcess();

        using(FileStream stream = File.OpenRead(path))
        {
            using(BinaryReader binaryData = new BinaryReader(stream))
            {
                _dataStrings.Add(binaryData.ReadString());
                while (binaryData.PeekChar() != -1)
                {
                    _dataStrings.Add(binaryData.ReadString());
                }
            }
        }

        OnReturnData?.Invoke(_dataStrings);
    }

    public void ClearData()
    {
        FileStream newFile = new FileStream(path, FileMode.Create);
        newFile.Close();
    }

    public static event ReturnDataHandler OnReturnData;
    public delegate void ReturnDataHandler(List<string> data);
}

public interface IObservable
{
    void GetData(List<string> listOfData);

    void WriteData(string data);
}
