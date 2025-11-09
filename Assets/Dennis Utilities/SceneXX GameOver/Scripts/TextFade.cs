using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TextFade : MonoBehaviour
{
    private YieldInstruction fadeInstruction = new YieldInstruction();
    public IEnumerator animateFont()
    {
        bool shouldAlternate = false;
        Text text = gameObject.GetComponent<Text>();
        while (true)
        {
            Color colorText = text.color; //Da text und flower selben alpha Kanal haben, ist egal ob text.color oder flower.color.
            float elapsedTime = 0.0f;
            float fadeTime = 2.25f;

            while ((elapsedTime < fadeTime) && !shouldAlternate)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                colorText.a = Mathf.Clamp01(elapsedTime / fadeTime);
                text.color = colorText;
            }
            shouldAlternate = true;
            elapsedTime = 0.0f;
            fadeTime = 2.25f;
            while ((elapsedTime < fadeTime) && shouldAlternate)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                colorText.a =  1.0f - Mathf.Clamp01(elapsedTime / fadeTime);
                text.color = colorText;
            }
            shouldAlternate = false;
        }
    }
}
