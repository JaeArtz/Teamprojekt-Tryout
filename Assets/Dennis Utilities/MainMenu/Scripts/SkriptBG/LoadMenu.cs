#if UNITY_EDITOR
using UnityEditor.PackageManager;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenu : MonoBehaviour
{
    public GameObject myLevelLoader;
    [SerializeField] private Notify loadingFailed;
    [SerializeField] private Notify deletingFailed;
    [SerializeField] private Notify deletingSucceded;

    [SerializeField]
    private GameObject backgroundAnimator;

    public GameObject quitUI;
    public GameObject mainUI;
    public GameObject newGameUI;
    public GameObject levelsUI;

    private AudioSource m_audioComponent;
    [SerializeField]
    private AudioClip m_soundEffectHover;
    [SerializeField]
    private AudioClip m_soundEffectClick;
    [SerializeField]
    private AudioClip m_blank;
    private void Awake()
    {
        m_audioComponent = GetComponent<AudioSource>();
        try
        {
            PlayerData data = SaveSystem.LoadData();
            if(!data.hasFinished)
            {
                StartCoroutine(backgroundAnimator.GetComponent<Main_Menu_Dark>().animateBGDark());
            }
            else
            {
                StartCoroutine(backgroundAnimator.GetComponent<Main_Menu_Light>().animateBGLight());
            }
        }
        catch (System.Exception e)
        {
            StartCoroutine(backgroundAnimator.GetComponent<Main_Menu_Dark>().animateBGDark());
        }
    }

    public void StartGame()
    {
        PlayerData data = SaveSystem.LoadData();
        if(data == null)
        {
            PlayClickSound();
            m_soundEffectHover = m_blank;
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            ClickSoundPlayNoDeactivation();
            CreateNewGame();
        }
    }

    //TODO: load the currently indexed game
    public void LoadGame()
    {
        try
        {
            PlayerData data = SaveSystem.LoadData();
            SaveSystem.AlterDataCheck(true);
            Debug.Log(data.currentScene);
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(data.currentScene);
            m_soundEffectHover = m_blank;
            PlayClickSound();
        }
        catch (System.Exception e)
        {
            if(NotifyManager.ManagerInstance != null)
            {
                NotifyManager.ManagerInstance.ShowNotification(loadingFailed);
            }
            ClickSoundPlayNoDeactivation();
        }
    }

    public void DeleteData()
    {
        try
        {
            PlayClickSound();
            DeleteSaveFile();
            myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
        }
        catch (System.Exception e)
        {

        }
    }

    //return to MainMenu and set currently indexed gamedata to 0
    public void Back()
    {
        m_soundEffectHover = m_blank;
        SaveSystem.SaveSelectedFileData(0);
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
    }
    public void DeleteSaveFile()
    {
        ClickSoundPlayNoDeactivation();

        quitUI.SetActive(true);
        mainUI.SetActive(false);
    }

    public void CreateNewGame()
    {
        try
        {
            newGameUI.SetActive(true);
            mainUI.SetActive(false);
        }
        catch (System.Exception e)
        {

        }
    }
    public void Yes()
    {
        try
        {
            SaveSystem.DeleteData();
            NotifyManager.ManagerInstance.ShowNotification(deletingSucceded);
        }
        catch (System.Exception e)
        {
            NotifyManager.ManagerInstance.ShowNotification(deletingFailed);
        }
        No();
        Debug.Log("Data deleted...");
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
    }

    public void No()
    {
        mainUI.SetActive(true);
        quitUI.SetActive(false);
    }

    public void YesGame()
    {
        int currentSave = SaveSystem.LoadSelectedFileData();
        SaveSystem.DeleteData();
        m_soundEffectHover = m_blank;
        SaveSystem.SaveSelectedFileData(currentSave);
        myLevelLoader.GetComponent<LevelLoaderScript>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void NoGame()
    {
        mainUI.SetActive(true);
        newGameUI.SetActive(false);
    }

    public void OpenLevelSelectionPanel()
    {
        ClickSoundPlayNoDeactivation();

        mainUI.SetActive(false);
        levelsUI.SetActive(true);
    }

    public void CloseLevelSelectionPanel()
    {
        ClickSoundPlayNoDeactivation();
        levelsUI.SetActive(false);
        mainUI.SetActive(true);
    }

    public void PlayHoverSound()
    {
        m_audioComponent.PlayOneShot(m_soundEffectHover);
    }

    public void PlayClickSound()
    {
        m_audioComponent.PlayOneShot(m_soundEffectClick);

        //TODO: waitForMillis instead of null value.
        m_soundEffectClick = m_blank;
    }

    public void ClickSoundPlayNoDeactivation()
    {
        m_audioComponent.PlayOneShot(m_soundEffectClick);
    }

}
