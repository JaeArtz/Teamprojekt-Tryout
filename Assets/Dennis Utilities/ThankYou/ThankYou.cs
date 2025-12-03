using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class ThankYou : MonoBehaviour
{
    private bool keyPressed = false;
    public TextFade text;

    void Start()
    {
        
        //https://discussions.unity.com/t/how-to-start-coroutine-from-another-script/115556/2
        StartCoroutine(text.GetComponent<TextFade>().animateFont());
        //yield return new WaitForSeconds(0.5f);
    }


    void Update()
    {
        //Allow Space Key Press to be done.
        if (Input.GetKey(KeyCode.Space) && !keyPressed)
        {
            SceneManager.LoadScene("MainMenu");
        }
        

    }

}