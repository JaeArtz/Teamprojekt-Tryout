using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main_Menu_Dark : MonoBehaviour
{
    
    public Sprite[] sprites;
    
    
    private int currentIndex = 0;
    private int lastIndex;
    private bool reverse = false;
    IEnumerator Start()
    {
        Image image = gameObject.GetComponent<Image>();
        lastIndex = sprites.Length - 1;
        while (true)
        {
            if(!reverse)
            {
                
                if (currentIndex == lastIndex)
                {
                    image.sprite = sprites[currentIndex];
                    reverse = true;
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    image.sprite = sprites[currentIndex];
                    currentIndex++;
                }
            }
            else
            {
                if (currentIndex == 0)
                {
                    image.sprite = sprites[currentIndex];
                    reverse = false;
                    yield return new WaitForSeconds(0.2f);
                }
                else
                {
                    image.sprite = sprites[currentIndex];
                    currentIndex--;
                }
            }
                
            yield return new WaitForSeconds(0.075f);
        }
    }
}
