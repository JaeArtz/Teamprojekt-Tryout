using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeGameState : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.SaveGameState(true);
        SceneManager.LoadScene("LoadedMenu");
    }
}
