using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LeafData
{
    public HashSet<int> leaves;

    public LeafData(HashSet<int> collectedLeaves)
    {
        leaves = collectedLeaves;
    }
}
