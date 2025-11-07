using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Backup_ChangeBackground : MonoBehaviour
{
    // You load the two images in here...
    public Sprite zero;
    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;
    public Sprite five;
    public Sprite six;
    public Sprite seven;

    IEnumerator Start()
    {
        Image image = gameObject.GetComponent<Image>();
        while (true)
        {
            image.sprite = zero;
            yield return new WaitForSeconds(0.5f);


            /*image.sprite = one;
            yield return new WaitForSeconds(0.5f);
            image.sprite = two;             
            yield return new WaitForSeconds(0.5f);
            image.sprite = three;           
            yield return new WaitForSeconds(0.5f);
            image.sprite = four;            
            yield return new WaitForSeconds(0.5f);
            image.sprite = five;            
            yield return new WaitForSeconds(0.5f);
            image.sprite = six;             
            yield return new WaitForSeconds(0.5f);
            image.sprite = seven;           
            yield return new WaitForSeconds(0.5f);
            image.sprite = six;             
            yield return new WaitForSeconds(0.5f);
            image.sprite = five;            
            yield return new WaitForSeconds(0.5f);
            image.sprite = four;            
            yield return new WaitForSeconds(0.5f);
            image.sprite = three;           
            yield return new WaitForSeconds(0.5f);
            image.sprite = two;             
            yield return new WaitForSeconds(0.5f);
            image.sprite = one;             
            yield return new WaitForSeconds(0.5f);*/
        }
    }
}
