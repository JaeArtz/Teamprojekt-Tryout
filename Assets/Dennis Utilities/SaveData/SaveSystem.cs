using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
public static class SaveSystem 
{
    public static void SaveSelectedFileData(int saveFile)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/selectedFile.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        SavefileSelectedData data = new SavefileSelectedData(saveFile);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static int LoadSelectedFileData()
    {
        string path = Application.persistentDataPath + "/selectedFile.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                stream.Close();
                return 0;
            }
            SavefileSelectedData savefile = formatter.Deserialize(stream) as SavefileSelectedData;
            stream.Close();
            return savefile.currentFile;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return 0;
        }

    }
    public static void SaveData(PlayerHealth player)
    {
        int currentSaveFile = LoadSelectedFileData();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/player{currentSaveFile}.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveSoulData(HashSet<string> collectedSouls)
    {
        int currentSaveFile = LoadSelectedFileData();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/collectedSouls{currentSaveFile}.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        SoulsData data = new SoulsData(collectedSouls);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static void SaveLeafData(HashSet<int> collectedLeaves)
    {
        int currentSaveFile = LoadSelectedFileData();
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + $"/collectedLeaves{currentSaveFile}.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        LeafData data = new LeafData(collectedLeaves);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadData()
    {
        int currentSaveFile = LoadSelectedFileData();
        string path = Application.persistentDataPath + $"/player{currentSaveFile}.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                stream.Close();
                return null;
            }
            PlayerData playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return playerData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }

    //Difference to LoadData(): LoadData(int) is called to check if current data for the Savefile exists.
    public static PlayerData LoadData(int currentSaveFile)
    {
        string path = Application.persistentDataPath + $"/player{currentSaveFile}.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                stream.Close();
                return null;
            }
            PlayerData playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return playerData;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }
    public static HashSet<string> LoadSoulData()
    {
        int currentSaveFile = LoadSelectedFileData();
        string path = Application.persistentDataPath + $"/collectedSouls{currentSaveFile}.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                stream.Close();
                return null;
            }
            SoulsData soulsData = formatter.Deserialize(stream) as SoulsData;
            stream.Close();
            return soulsData.souls;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }
    public static HashSet<int> LoadLeafData()
    {
        int currentSaveFile = LoadSelectedFileData();
        string path = Application.persistentDataPath + $"/collectedLeaves{currentSaveFile}.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            if (stream.Length == 0)
            {
                stream.Close();
                return null;
            }
            LeafData leavesData = formatter.Deserialize(stream) as LeafData;
            stream.Close();
            return leavesData.leaves;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }
    public static void DeleteData()
    {
        int currentSaveFile = LoadSelectedFileData();

        string path = Application.persistentDataPath + $"/player{currentSaveFile}.bin";
        string soulsPath = Application.persistentDataPath + $"/collectedSouls{currentSaveFile}.bin";
        string leafPath = Application.persistentDataPath + $"/collectedLeaves{currentSaveFile}.bin";
        bool isAlreadyDeleted = false;
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogError("Save file already deleted " + path);
            isAlreadyDeleted = true;
        }
        if (File.Exists(soulsPath))
        {
            File.Delete(soulsPath);
        }
        else
        {
            Debug.LogError("Souls file is already deleted " + soulsPath);
            isAlreadyDeleted = true;
        }
        if (File.Exists(leafPath))
        {
            File.Delete(leafPath);
        }
        else
        {
            Debug.LogError("Leaf file is already deleted " + leafPath);
            isAlreadyDeleted = true;
        }
        if(isAlreadyDeleted)
        {
            throw new System.Exception();
        }

        //Resets currently set Savefile indexer
        SaveSelectedFileData(0);

    }
    /// <summary>
    /// First loads current State of Data and then changes the current loadState depending on the transmitted state.
    /// </summary>
    /// <param name="loadState"></param>
    public static void AlterDataCheck(bool loadState)
    {
        int currentSaveFile = LoadSelectedFileData();
        string path = Application.persistentDataPath + $"/player{currentSaveFile}.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            stream = new FileStream(path, FileMode.Create);
            PlayerData data = new PlayerData(playerData, loadState);
            formatter.Serialize(stream, data);
            stream.Close();
        }
        else
        {
            Debug.LogError("Save file not found in " + path);

        }
    }
}
