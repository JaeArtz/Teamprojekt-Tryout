using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShowStoryAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject NotifyPanel;
    [SerializeField]
    private GameObject StoryImage; 
    [SerializeField]
    public GameObject StoryText;
    void Start()
    {
        NotifyPanel.SetActive(false);
    }


    public IEnumerator ShowCollectedAnimation(string message)
    {
        StoryText.GetComponent<TMP_Text>().text = message;
        
        Color imageColor = StoryImage.GetComponent<Image>().color;
        imageColor.a = 0.75f;
        StoryImage.GetComponent<Image>().color = imageColor;
        Color storyColor = StoryText.GetComponent<TMP_Text>().color;
        storyColor.a = 0.75f;
        StoryText.GetComponent<TMP_Text>().color = storyColor;

        yield return new WaitForSeconds(2.0f);

        imageColor = StoryImage.GetComponent<Image>().color;
        imageColor.a = 0;
        StoryImage.GetComponent<Image>().color = imageColor;
        storyColor = StoryText.GetComponent<TMP_Text>().color;
        storyColor.a = 0;
        StoryText.GetComponent<TMP_Text>().color = storyColor;

        NotifyPanel.SetActive(false);
    }
}
