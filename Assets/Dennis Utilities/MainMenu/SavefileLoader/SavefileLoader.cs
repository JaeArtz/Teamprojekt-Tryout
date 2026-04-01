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

    private TMP_Text savefileText1;
    private TMP_Text savefileText2;
    private TMP_Text savefileText3;
    private void Awake()
    {
        myLevelLoader = GameObject.Find("LevelLoader");

        button1 = GameObject.Find("Button_Savefile_1");
        button2 = GameObject.Find("Button_Savefile_2");
        button3 = GameObject.Find("Button_Savefile_3");

        buttonText1 = GameObject.Find("Button_Savefile_1_Text");
        buttonText2 = GameObject.Find("Button_Savefile_2_Text");
        buttonText3 = GameObject.Find("Button_Savefile_3_Text");

        savefileText1 = GameObject.Find("Savestate1_Text").GetComponent<TextMeshPro>();
        savefileText2 = GameObject.Find("Savestate2_Text").GetComponent<TextMeshPro>();
        savefileText3 = GameObject.Find("Savestate3_Text").GetComponent<TextMeshPro>();
    }

    // Update is called once per frame

    private void Start()
    {
        //TODO: Ask if SaveData exists and if it does update their Information.
    }

    public void ReturnToMenu()
    {
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
    }

    public void SelectSaveFile1()
    {
        //Set Current selected file to 1
    }

    public void SelectSaveFile2()
    {
        //Set Current selected file to 2
    }

    public void SelectSaveFile3()
    {
        //Set Current selected file to 3
    }

    public void LoadAlterMainMenu()
    {
        //TODO...
    }
}
