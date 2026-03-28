using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
//Quelle: https://stackoverflow.com/questions/39848911/unity-fade-image-alpha-over-time
public class GateFade : MonoBehaviour
{
    private bool keyPressed = false;
    private bool hasFadePassed = false;
    private YieldInstruction fadeInstruction = new YieldInstruction();
    IEnumerator Start()
    {
        //Fade Away of the Player Character!
        SpriteRenderer gate = gameObject.GetComponent<SpriteRenderer>();
        Color colorPlayer = gate.color;
        float elapsedTime = 0.0f;
        float fadeTime = 2.0f;
        while (elapsedTime < fadeTime)
        {
            yield return fadeInstruction;
            elapsedTime += Time.deltaTime;
            colorPlayer.a = 1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
            gate.color = colorPlayer;
        }
        hasFadePassed = true;
    }
}