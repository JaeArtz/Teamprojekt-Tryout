using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
//Quelle: https://stackoverflow.com/questions/39848911/unity-fade-image-alpha-over-time
public class FadeAway : MonoBehaviour
{
    private bool keyPressed = false;
    private bool hasFadePassed = false;
    public TextFade text;
    public Image flower;
    private YieldInstruction fadeInstruction = new YieldInstruction();
    IEnumerator Start()
    {
        //Fade Away of the Player Character!
        Image player = gameObject.GetComponent<Image>();
        Color colorPlayer = player.color;
        float elapsedTime = 0.0f;
        float fadeTime = 3.0f;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            colorPlayer.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            player.color = colorPlayer;
        }
        hasFadePassed = true;

        //TODO: Inserting "Press Any Key to Continue" Animation.
        Color colorFlower = flower.color; //Da text und flower selben alpha Kanal haben, ist egal ob text.color oder flower.color.
        elapsedTime = 0.0f;
        fadeTime = 1.0f;

        //https://discussions.unity.com/t/how-to-start-coroutine-from-another-script/115556/2
        StartCoroutine(text.GetComponent<TextFade>().animateFont());
        
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            colorFlower.a = Mathf.Clamp01(elapsedTime / fadeTime);
            flower.color = colorFlower;
        }
    }

    
    void Update()
    {
        
        //Allow Any Key Press to be done.
        if (hasFadePassed)
        {
            if (Input.anyKey && !keyPressed)
            {
                SceneManager.LoadScene("MainMenu");  
            }
        }
        
    }

}
