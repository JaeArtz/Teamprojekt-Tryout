using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData 
{
    public HashSet<int> levels;

    public LevelData(HashSet<int> unlockedLevels)
    {
        levels = unlockedLevels;
    }
}
