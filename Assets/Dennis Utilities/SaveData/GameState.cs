using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public bool gameState;

    public GameState(bool currentState)
    {
        gameState = currentState;
    }
}
