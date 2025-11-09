using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeBackground : MonoBehaviour
{
    /*Containers for the Background Images.*/
    public Sprite[] background_dark_tree;
    public Sprite[] background_shimmering_tree;
    public Sprite[] background_reviving_tree;
    public Sprite[] background_glowing_tree;

    /*Container we want to set.*/
    private Sprite[] sprites;
    
    /*private Attributes.*/
    private int currentBackground;
    private int currentIndex = 0;
    private int lastIndex;
    private bool reverse = false;

    IEnumerator Start()
    {
        ChooseBackground();
        Image image = gameObject.GetComponent<Image>();
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
    
    /// <summary>
    /// TODO: Change the Selection Criteria from Random to collected Goodies.
    /// </summary>
    private void ChooseBackground()
    {
        currentBackground = Random.Range(0, 4);
        switch (currentBackground)
        {
            case 0:
                sprites = background_dark_tree;
                break;
            case 1:
                sprites = background_shimmering_tree;
                break;
            case 2:
                sprites = background_reviving_tree;
                break;
            case 3:
                sprites = background_glowing_tree;
                break;
        }
    }
}
