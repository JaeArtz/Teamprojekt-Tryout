using NUnit.Framework;
using UnityEngine;
[System.Serializable]
public class SavefileSelectedData
{
    public int currentFile = 0;

    public SavefileSelectedData(int fileNumber)
    {
        currentFile = fileNumber;
    }

}
