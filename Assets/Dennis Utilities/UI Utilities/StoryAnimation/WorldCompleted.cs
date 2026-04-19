using System.Collections;
using UnityEngine;

public class WorldCompleted : MonoBehaviour
{
    private GameObject levelManager;
    private GameObject notifyPanel;
    [SerializeField]
    private string message;
    void Awake()
    {
        levelManager = GameObject.Find("GameManager");
        notifyPanel = GameObject.Find("NotifyPanel");
    }

    private void Start()
    {
        
    }
    private void Update()
    {
        if (!levelManager.GetComponent<LevelManager>().wasAlreadyVisited)
        {
            StartCoroutine(processAnimation());
            levelManager.GetComponent<LevelManager>().wasAlreadyVisited = true;
        }
    }

    IEnumerator processAnimation()
    {
        notifyPanel.SetActive(true);
        Debug.Log(message);
        StartCoroutine(notifyPanel.GetComponent<ShowStoryAnimation>().ShowCollectedAnimation(message));
        yield return new WaitForSeconds(2.0f);
        this.enabled = false;
    }

}
