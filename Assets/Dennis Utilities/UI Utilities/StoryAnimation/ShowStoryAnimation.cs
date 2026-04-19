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

    private YieldInstruction fadeInstruction = new YieldInstruction();
    void Start()
    {
        NotifyPanel.SetActive(false);
    }


    public IEnumerator ShowCollectedAnimation(string message)
    {
        StoryText.GetComponent<TMP_Text>().text = message;
        
        /* Color imageColor = StoryImage.GetComponent<Image>().color;
        imageColor.a = 0.66f;
        StoryImage.GetComponent<Image>().color = imageColor;
        Color storyColor = StoryText.GetComponent<TMP_Text>().color;
        storyColor.a = 1.0f;
        StoryText.GetComponent<TMP_Text>().color = storyColor;*/

        float elapsedTime = 0.0f;
        float fadeTime = 1.5f;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            Color imageColor = StoryImage.GetComponent<Image>().color;
            imageColor.a = Mathf.Clamp01(elapsedTime / fadeTime);
            imageColor.a *= 0.66f;
            StoryImage.GetComponent<Image>().color = imageColor;
            Color storyColor = StoryText.GetComponent<TMP_Text>().color;
            storyColor.a = Mathf.Clamp01(elapsedTime / fadeTime);
            StoryText.GetComponent<TMP_Text>().color = storyColor;
        }
        elapsedTime = 0.0f;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            Color imageColor = StoryImage.GetComponent<Image>().color;
            imageColor.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            imageColor.a *= 0.66f;
            StoryImage.GetComponent<Image>().color = imageColor;
            Color storyColor = StoryText.GetComponent<TMP_Text>().color;
            storyColor.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            StoryText.GetComponent<TMP_Text>().color = storyColor;
        }
        /*imageColor = StoryImage.GetComponent<Image>().color;
        imageColor.a = 0;
        StoryImage.GetComponent<Image>().color = imageColor;
        storyColor = StoryText.GetComponent<TMP_Text>().color;
        storyColor.a = 0;
        StoryText.GetComponent<TMP_Text>().color = storyColor;*/

        NotifyPanel.SetActive(false);
    }
}
