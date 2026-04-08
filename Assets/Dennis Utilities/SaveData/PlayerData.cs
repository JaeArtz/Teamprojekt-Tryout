using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class PlayerData
{
    public int currentLives;

    public float[] currentPosition;

    public string currentScene;

    public bool wasLoaded = false;
    public PlayerData(PlayerHealth player)
    {
        currentLives = player.currentHealth;
        currentPosition = new float[3];
        currentPosition[1] = player.transform.localPosition.y;
        currentPosition[2] = player.transform.localPosition.z;
        currentPosition[0] = player.transform.localPosition.x;
        currentScene = player.currentScene;
    }

    public PlayerData(PlayerData player, bool loaded)
    {
        currentLives = player.currentLives;
        currentPosition = new float[3];
        currentPosition[0] = player.currentPosition[0];
        currentPosition[1] = player.currentPosition[1];
        currentPosition[2] = player.currentPosition[2];
        currentScene = player.currentScene;
        wasLoaded = loaded;
    }

    public PlayerData(PlayerHealth player, PlayerRespawn respawn)
    {
        currentLives = player.currentHealth;
        currentPosition = new float[3];
        currentPosition[0] = respawn.respawnPoint.x;
        currentPosition[1] = respawn.respawnPoint.y;
        currentPosition[2] = respawn.respawnPoint.z;
        currentScene = player.currentScene;
    }

}
