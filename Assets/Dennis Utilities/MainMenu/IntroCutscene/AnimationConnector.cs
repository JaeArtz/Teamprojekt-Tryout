using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
// using static UnityEditor.Experimental.GraphView.GraphView;
public class AnimationConnector : MonoBehaviour
{
    private bool keyPressed = false;

    private GameObject Introduction;
    private GameObject StoryTreeFirstPart;
    private GameObject StoryTreeSecondPart;
    private GameObject StoryHeartBeatSection;
    private GameObject FinalScreen;


    [SerializeField]
    private AudioClip audio_storyTreeFirstPart;
    [SerializeField]
    private AudioClip audio_storyTreeSecondPart;
    [SerializeField]
    private AudioClip audio_storyHeartBeatSection;
    [SerializeField]
    private AudioClip audio_goldenDroplett;
    [SerializeField]
    private AudioClip heartBeat;

    private AudioSource m_audioComponent;
    private GameObject LevelLoader;
    private void Awake()
    {
        m_audioComponent = GetComponent<AudioSource>();
        LevelLoader = GameObject.Find("LevelLoader");
        Introduction = GameObject.Find("Introduction");
        StoryTreeFirstPart = GameObject.Find("StoryTreeFirstPart");
        StoryTreeSecondPart = GameObject.Find("StoryTreeSecondPart");
        StoryHeartBeatSection = GameObject.Find("StoryHeartBeatSection");
        FinalScreen = GameObject.Find("FinalScreen");

        Introduction.SetActive(false);
        StoryTreeFirstPart.SetActive(false);
        StoryTreeSecondPart.SetActive(false);
        StoryHeartBeatSection.SetActive(false);
        FinalScreen.SetActive(false);
    }
    void Start()
    {
        StartCoroutine(AnimateIntro());
    }

    IEnumerator AnimateIntro()
    {
        Introduction.SetActive(true);
        Introduction.GetComponent<Animator>().Play("Introduction");
        yield return new WaitForSeconds(7.0f);
        Introduction.SetActive(false);


        StoryTreeFirstPart.SetActive(true);
        StoryTreeFirstPart.GetComponent<Animator>().Play("Withering");
        m_audioComponent.PlayOneShot(audio_storyTreeFirstPart);
        yield return new WaitForSeconds(32.0f);
        StoryTreeFirstPart.SetActive(false);

        StoryTreeSecondPart.SetActive(true);
        m_audioComponent.PlayOneShot(audio_storyTreeSecondPart);
        StoryTreeSecondPart.GetComponent<Animator>().Play("deadTree");
        yield return new WaitForSeconds(14.0f);
        StoryTreeSecondPart.SetActive(false);

        StoryHeartBeatSection.SetActive(true);
        StoryHeartBeatSection.GetComponent<Animator>().Play("HeartBeat");
        m_audioComponent.PlayOneShot(audio_storyHeartBeatSection);
        yield return new WaitForSeconds(24.0f);
        m_audioComponent.PlayOneShot(audio_goldenDroplett);
        yield return new WaitForSeconds(4.0f);
        StoryHeartBeatSection.SetActive(false);
        FinalScreen.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        m_audioComponent.PlayOneShot(heartBeat);
        yield return new WaitForSeconds(4.5f);

        try
        {
            if (SaveSystem.LoadSelectedFileData() == 0)
            {
                LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
            }
            else
            {
                LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("LoadedMenu");
            }
        }
        catch (System.Exception error)
        {
            SaveSystem.SaveSelectedFileData(0);
            LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey && !keyPressed)
        {
            try
            {
                if (SaveSystem.LoadSelectedFileData() == 0)
                {
                    LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
                }
                else
                {
                    LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("LoadedMenu");
                }
            }
            catch (System.Exception error)
            {
                SaveSystem.SaveSelectedFileData(0);
                LevelLoader.GetComponent<LevelLoaderScript>().LoadScene("MainMenu");
            }
        }
    }
}
