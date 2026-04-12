using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main_Menu_Light : MonoBehaviour
{

    public Sprite[] sprites;


    private int currentIndex = 0;
    private int lastIndex;
    private bool reverse = false;

    
    public IEnumerator animateBGLight()
    {
        Image image = gameObject.GetComponent<Image>();
        RectTransform rt = image.GetComponent(typeof(RectTransform)) as RectTransform;
        float y = rt.localScale.y;
        rt.localScale = new Vector3(rt.localScale.x, 1.75f, rt.localScale.z);

        lastIndex = sprites.Length - 1;
        while (true)
        {
            if (!reverse)
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
