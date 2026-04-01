using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoulsData 
{
    public HashSet<string> souls;

    public SoulsData(HashSet<string> collectedSouls)
    {
        souls = collectedSouls;
    }
}
