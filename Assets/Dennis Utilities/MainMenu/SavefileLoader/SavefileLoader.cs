using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavefileLoader : MonoBehaviour
{
    private GameObject myLevelLoader;

    private GameObject button1;
    private GameObject button2;
    private GameObject button3;

    private GameObject buttonText1;
    private GameObject buttonText2;
    private GameObject buttonText3;

    private GameObject savefileText1;
    private GameObject savefileText2;
    private GameObject savefileText3;
    private void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");

        button1 = GameObject.Find("Button_Savefile_1");
        button2 = GameObject.Find("Button_Savefile_2");
        button3 = GameObject.Find("Button_Savefile_3");

        buttonText1 = GameObject.Find("Button_Savefile_1_Text");
        buttonText2 = GameObject.Find("Button_Savefile_2_Text");
        buttonText3 = GameObject.Find("Button_Savefile_3_Text");

        savefileText1 = GameObject.Find("Savestate1_Text");
        savefileText2 = GameObject.Find("Savestate2_Text");
        savefileText3 = GameObject.Find("Savestate3_Text");
    }

    // Update is called once per frame

    private void Start()
    {
        PlayerData player1 = SaveSystem.LoadData(1);
        PlayerData player2 = SaveSystem.LoadData(2);
        PlayerData player3 = SaveSystem.LoadData(3);
        if (player1 != null)
        {
            buttonText1.GetComponent<TextMeshProUGUI>().text = player1.currentScene;
            int collectedSouls = SaveSystem.LoadSoulData(1).Count;
            int collectedLeaves = SaveSystem.LoadLeafData(1).Count; 
            savefileText1.GetComponent<TMP_Text>().text = $"Collected Souls: {collectedSouls}/4 \nCollected Leaves {collectedLeaves }/22";
        }
        else
        {
            buttonText1.GetComponent<TextMeshProUGUI>().text = "Empty";
            savefileText1.GetComponent<TMP_Text>().text = "Collected Souls: 0/4 \nCollected Leaves 0/22";
        }

        if (player2 != null)
        {
            buttonText2.GetComponent<TextMeshProUGUI>().text = player2.currentScene;
            int collectedSouls = SaveSystem.LoadSoulData(2).Count;
            int collectedLeaves = SaveSystem.LoadLeafData(2).Count;
            savefileText2.GetComponent<TMP_Text>().text = $"Collected Souls: {collectedSouls}/4 \nCollected Leaves {collectedLeaves}/22";
        }
        else
        {
            buttonText2.GetComponent<TextMeshProUGUI>().text = "Empty";
            savefileText2.GetComponent<TMP_Text>().text = "Collected Souls: 0/4 \nCollected Leaves 0/22";
        }

        if (player3 != null)
        {
            buttonText3.GetComponent<TextMeshProUGUI>().text = player3.currentScene;
            int collectedSouls = SaveSystem.LoadSoulData(3).Count;
            int collectedLeaves = SaveSystem.LoadLeafData(3).Count;
            savefileText3.GetComponent<TMP_Text>().text = $"Collected Souls: {collectedSouls}/4 \nCollected Leaves {collectedLeaves}/22";
        }
        else
        {
            buttonText3.GetComponent<TextMeshProUGUI>().text = "Empty";
            savefileText3.GetComponent<TMP_Text>().text = "Collected Souls: 0/4 \nCollected Leaves 0/22";
        }
    }

    public void ReturnToMenu()
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
    }

    public void SelectSaveFile1()
    {
        //Set Current selected file to 1
        SaveSystem.SaveSelectedFileData(1);
        LoadAlterMainMenu();
    }

    public void SelectSaveFile2()
    {
        //Set Current selected file to 2
        SaveSystem.SaveSelectedFileData(2);
        LoadAlterMainMenu();
    }

    public void SelectSaveFile3()
    {
        //Set Current selected file to 3
        SaveSystem.SaveSelectedFileData(3);
        LoadAlterMainMenu();
    }

    public void LoadAlterMainMenu()
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("LoadedMenu");
    }
}
