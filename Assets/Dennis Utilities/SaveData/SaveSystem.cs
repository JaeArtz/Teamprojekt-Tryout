using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystem 
{
    public static void SaveData(PlayerHealth player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/souls.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadData()
    {
        string path = Application.persistentDataPath + "/souls.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

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

    public static void DeleteData()
    {
        string path = Application.persistentDataPath + "/souls.fun";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        else
        {
            Debug.LogError("Save file already deleated " + path);
           
        }

    }
    /// <summary>
    /// First loads current State of Data and then changes the current loadState depending on the transmitted state.
    /// </summary>
    /// <param name="loadState"></param>
    public static void AlterDataCheck(bool loadState)
    {
        string path = Application.persistentDataPath + "/souls.fun";
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
